﻿namespace Unosquare.Labs.EmbedIO.BearerToken
{
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System;
#if NET46
    using System.Net;
#else
    using Unosquare.Net;
#endif

    /// <summary>
    /// Context to share data with AuthorizationServerProvider
    /// </summary>
    public class ValidateClientAuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateClientAuthenticationContext"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <exception cref="System.ArgumentNullException">httpContext</exception>
        public ValidateClientAuthenticationContext(HttpListenerContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            HttpContext = httpContext;

            // TODO: Add Claims
            StandardClaims = new ClaimsIdentity();
        }

        /// <summary>
        /// The Client Id
        /// </summary>
        public string ClientId { get; protected set; }

        /// <summary>
        /// Flags if the Validation has errors
        /// </summary>
        public bool HasError { get; protected set; }

        /// <summary>
        /// Indicates if the Validation is right
        /// </summary>
        public bool IsValidated { get; protected set; }

        /// <summary>
        /// Http Context instance
        /// </summary>
        public HttpListenerContext HttpContext { get; protected set; }

        /// <summary>
        /// Claims
        /// </summary>
        public ClaimsIdentity StandardClaims { get; set; }

        /// <summary>
        /// Rejects a validation
        /// </summary>
        public void Rejected()
        {
            IsValidated = false;
            HasError = true;
        }

        /// <summary>
        /// Validates credentials with clientId
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void Validated(string clientId)
        {
            ClientId = clientId;
            Validated();
        }

        /// <summary>
        /// Validates credentials
        /// </summary>
        public void Validated()
        {
            IsValidated = true;
            HasError = false;
        }

        /// <summary>
        /// Retrieve JsonWebToken
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <returns></returns>
        public string GetToken(SymmetricSecurityKey secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = StandardClaims,
                SigningCredentials = new SigningCredentials(secretKey,
                    SecurityAlgorithms.HmacSha256Signature)
            });
            return tokenHandler.WriteToken(plainToken);
        }
    }
}