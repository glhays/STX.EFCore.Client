﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using STX.EFCore.Client.Services.Foundations.Operations;
using STX.EFCore.Client.Tests.Unit.Brokers.Storages;
using STX.EFCore.Client.Tests.Unit.Models.Foundations.Users;

namespace STX.EFCore.Client.Tests.Unit.Services.Foundations.Operations
{
    public partial class OperationServiceTests
    {
        [Fact]
        public async Task BulkDeleteAsyncShouldDetachAllEntitiesWhenExceptionIsThrown()
        {
            // Given
            IEnumerable<User> randomUsers = CreateRandomUsers();
            IEnumerable<User> inputUsers = randomUsers;
            List<EntityState?> statesAfterExplicitDetach = new List<EntityState?>();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            TestDbContext dbContext = new TestDbContext(options);
            OperationService operationService = new OperationService(dbContext);
            Exception errorException = new Exception("Database error");
            Exception expectedException = errorException.DeepClone();
            await dbContext.BulkInsertAsync(inputUsers);
            bool firstTime = true;

            dbContext.SavingChanges += (sender, e) =>
            {
                if (firstTime)
                {
                    firstTime = false;
                    throw errorException;
                }
            };

            // When
            ValueTask bulkUpdateUserTask = operationService.BulkDeleteAsync(inputUsers);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(
                    bulkUpdateUserTask.AsTask);

            foreach (var user in inputUsers)
            {
                statesAfterExplicitDetach.Add(dbContext.Entry(user).State);
            }

            // Then
            actualException.Message.Should().BeEquivalentTo(expectedException.Message);
            statesAfterExplicitDetach.Should().AllBeEquivalentTo(EntityState.Detached);
            await dbContext.BulkDeleteAsync(inputUsers);
        }
    }
}
