﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using STX.EFCore.Client.Brokers.Storages;
using STX.EFCore.Client.Services.Foundations.Operations;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;
using Tynamix.ObjectFiller;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly OperationService operationService;

        public OperationServiceTests()
        {
            storageBrokerMock = new Mock<IStorageBroker>();
            this.operationService = new OperationService(storageBrokerMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string CreateRandomString() =>
            new MnemonicString().GetValue();

        private static List<User> CreateRandomUsers() =>
            CreateUserFiller().Create(count: GetRandomNumber()).ToList();

        private static User CreateRandomUser() =>
            CreateUserFiller().Create();

        private static Filler<User> CreateUserFiller()
        {
            var filler = new Filler<User>();
            filler.Setup().OnProperty(user => user.Id).Use(() => Guid.NewGuid());

            return filler;
        }
    }
}
