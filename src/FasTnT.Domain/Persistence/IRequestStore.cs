﻿using FasTnT.Model;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IRequestStore
    {
        Task<Guid> Store(EpcisRequestHeader request);
    }
}