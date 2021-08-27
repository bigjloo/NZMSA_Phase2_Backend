using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Data;
using NZMSA_HYD.Data;
using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using HotChocolate.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Core;
using NZMSA_HYD.KeyVault;

namespace NZMSA_HYD.GraphQL.Mutations.Users
{   
    [ExtendObjectType(name: "Mutation")]
    public class UserMutation
    {
        [UseDbContext(typeof(AppDbContext))]
        public async Task<LoginPayload> LoginAsync(
            LoginInput input,
            [ScopedService] AppDbContext context,
            CancellationToken cancellationToken)
        {
            var githubClientIDStr = AzureKeyVault.GetKey(Startup.Configuration["KeyVault:GithubId"]);
            var githubClientSecretStr = AzureKeyVault.GetKey(Startup.Configuration["KeyVault:GithubSecret"]);
            //var githubClientIDStr = Startup.Configuration["Github:ClientId"];
            //var githubClientSecretStr = Startup.Configuration["Github:ClientSecret"];
            var request = new OauthTokenRequest(githubClientIDStr, githubClientSecretStr, input.Code);

            var client = new GitHubClient(new ProductHeaderValue("HYD"));
            var tokenInfo = await client.Oauth.CreateAccessToken(request);

            if (tokenInfo.AccessToken == null)
            {
                throw new GraphQLRequestException(
                    ErrorBuilder.New()
                        .SetMessage("bad code")
                        .SetCode("AUTH_NOT_AUTHENTICATED")
                        .Build()
                    );
            }

            // use AccessToken to get currentUser
            client.Credentials = new Credentials(tokenInfo.AccessToken);
            var currentUser = await client.User.Current();

            // Find user in our database by the Github login 
            var user = await context.Users.FirstOrDefaultAsync(u => u.Github == currentUser.Login, cancellationToken);

            // If user does not exist in database, create new User from github fetched info 
            if (user == null)
            {
                user = new Model.User
                {
                    Name = currentUser.Name ?? currentUser.Login,
                    Github = currentUser.Login,
                    ImageURI = currentUser.AvatarUrl,
                };

                context.Users.Add(user);
                await context.SaveChangesAsync(cancellationToken);
            }
            //authentication successful so generate jwt token
            var JWTSecret = AzureKeyVault.GetKey(Startup.Configuration["KeyVault:JWTSecret"]);
            //var JWTSecret = Startup.Configuration["JWT:Secret"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSecret));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Add userId to claims list for authorization 
            var claims = new List<Claim>{
                new Claim("userId", user.Id.ToString()), 
            };

            // returns a JWT token which can be used to login, expires in 90 days
            var jwtToken = new JwtSecurityToken(
                "HYD",
                "NZMSA-2021",
                claims,
                expires: DateTime.Now.AddDays(90),
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new LoginPayload(user, token);
        }
    }
}
