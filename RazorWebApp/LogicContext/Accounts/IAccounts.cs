﻿using System;
using System.Threading;
using System.Threading.Tasks;
using RazorWebApp.Shared.Dto;

namespace RazorWebApp.LogicContext.Accounts
{
    public interface IAccounts
    {
        /// <summary>
        /// Perform sign-up action for given PayLoad and Password Salt (recommended value is > 10).
        /// </summary>
        /// <param name="APayLoad"></param>
        /// <param name="APasswordSalt"></param>
        /// <param name="ACancellationToken"></param>
        /// <returns></returns>
        Task<int> SignUp(UserCreateDto APayLoad, int APasswordSalt, CancellationToken ACancellationToken = default);

        /// <summary>
        /// Perform sign-in action and log it to the history table.
        /// </summary>
        /// <param name="AEmailAddr"></param>
        /// <param name="APassword"></param>
        /// <param name="ACancellationToken"></param>
        /// <returns></returns>
        Task<(Guid SessionId, bool IsSignedIn, bool IsExisting)> SignIn(string AEmailAddr, string APassword, CancellationToken ACancellationToken = default);
    }
}
