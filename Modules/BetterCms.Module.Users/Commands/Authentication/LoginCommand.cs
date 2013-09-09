﻿using System;
using System.Linq;
using System.Web;
using System.Web.Security;

using BetterCms.Core.Exceptions;
using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Mvc.Commands;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Users.Models;
using BetterCms.Module.Users.Services;
using BetterCms.Module.Users.ViewModels.Authentication;
using BetterCms.Module.Users.ViewModels.Registration;

namespace BetterCms.Module.Users.Commands.Authentication
{
    public class LoginCommand : CommandBase, ICommand<LoginViewModel, HttpCookie>
    {
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginCommand" /> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public LoginCommand(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        /// <summary>
        /// Executes a command to save role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Response with id and version
        /// </returns>
        public HttpCookie Execute(LoginViewModel request)
        {
            if (Membership.ValidateUser(request.UserName, request.Password))
            {
                if (!Roles.Enabled)
                {
                    throw new CmsException("A roles provider should be enabled in web.config.");
                }

                var roles = Roles.GetRolesForUser(request.UserName);
                var authTicket = new FormsAuthenticationTicket(1, request.UserName, DateTime.Now, DateTime.Now.AddMonths(1), request.RememberMe, string.Join("|", roles));

                var cookieContents = FormsAuthentication.Encrypt(authTicket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents) {
                                                                                                     Expires = authTicket.Expiration,
                                                                                                     Path = FormsAuthentication.FormsCookiePath
                                                                                                 };
                return cookie;
            }

            return null;
        }
    }
}