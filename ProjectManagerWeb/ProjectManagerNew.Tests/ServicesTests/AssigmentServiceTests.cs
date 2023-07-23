using BLL.Interfaces;
using BLL.Services;
using Core.Models;
using DAL.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerNew.Tests.ServicesTests
{
    public class AssigmentServiceTests
    {
        [Fact]
        public async void AssigmentService_CreateAssigment_ResultTrue()
        {
            //Arrange
            var assigment = new Assigment()
            {
                Name = "Test",
                Description = "Test",
                CurrentProjectId = Guid.NewGuid()
            };
            var responsibleUsers = new List<string>() { Guid.NewGuid().ToString() };

            var repositoryMock = new Mock<IUnitOfWork>();
            var userAuthenticationServiceMock = new Mock<IUserAuthenticationService>();
            var emailServiceMock = new Mock<IEmailService>();

            var assigmentService = new AssigmentService(repositoryMock.Object, userAuthenticationServiceMock.Object, emailServiceMock.Object);

            //repositoryMock.Setup(x => x.Projects.GetById(It.IsAny<Guid>())).Returns(new Task<Result<Project>>(() => new Result<Project>(true, new Project())));
            //repositoryMock.Setup(x => x.Assigments.Add(assigment)).Returns(new Task<Result<Assigment>>(() => new Result<Assigment>(true)));
            userAuthenticationServiceMock.Setup(x => x.GetUserById(It.IsAny<Guid>().ToString())).Returns(new Task<Result<AppUser>>(() => new Result<AppUser>(true, new AppUser())));
            //repositoryMock.Setup(x => x.UserAssigments.AddRange(new List<UserAssigment>())).Returns(new Task<Result<UserAssigment>>(() => new Result<UserAssigment>(true)));
            repositoryMock.Setup(x => x.Complete()).Returns(() => 1);

            //Act
            var result = assigmentService.CreateAssigment(assigment, responsibleUsers);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<Result<bool>>>();
            result.Should().BeEquivalentTo(new Result<bool>(true));
        }
    }
}
