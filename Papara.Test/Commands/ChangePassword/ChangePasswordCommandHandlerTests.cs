using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Papara.Business.Commands.ChangePassword;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Session;
using Papara.Data.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Commands.ChangePassword
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ISessionContext> _sessionContextMock;
        private readonly Mock<IValidator<ChangePasswordRequest>> _validatorMock;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );

            _sessionContextMock = new Mock<ISessionContext>();
            _validatorMock = new Mock<IValidator<ChangePasswordRequest>>();

            _handler = new ChangePasswordCommandHandler(
                _userManagerMock.Object,
                _sessionContextMock.Object,
                _validatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            var command = new ChangePasswordCommand(changePasswordRequest); // Updated

            var user = new User { Email = "user@example.com" };
            var userResponse = new User { Email = "user@example.com" };

            _validatorMock.Setup(v => v.ValidateAsync(changePasswordRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionContextMock.Setup(s => s.HttpContext.User)
                               .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                               {
                               new Claim(ClaimTypes.Email, "user@example.com")
                               })));

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email))
                            .ReturnsAsync(userResponse);
            _userManagerMock.Setup(um => um.ChangePasswordAsync(userResponse, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Password changed successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ReturnsErrorResponse()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            var command = new ChangePasswordCommand(changePasswordRequest); // Updated

            _validatorMock.Setup(v => v.ValidateAsync(changePasswordRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                          {
                          new FluentValidation.Results.ValidationFailure("OldPassword", "OldPassword is required.")
                          }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("OldPassword is required.", result.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            var command = new ChangePasswordCommand(changePasswordRequest); // Updated

            _validatorMock.Setup(v => v.ValidateAsync(changePasswordRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionContextMock.Setup(s => s.HttpContext.User)
                               .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                               {
                               new Claim(ClaimTypes.Email, "user@example.com")
                               })));

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Login failed.", result.Message);
        }

        [Fact]
        public async Task Handle_UserNotFoundByEmail_ReturnsErrorResponse()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            var command = new ChangePasswordCommand(changePasswordRequest); // Updated

            var user = new User { Email = "user@example.com" };

            _validatorMock.Setup(v => v.ValidateAsync(changePasswordRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionContextMock.Setup(s => s.HttpContext.User)
                               .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                               {
                               new Claim(ClaimTypes.Email, "user@example.com")
                               })));

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email))
                            .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Login failed.", result.Message);
        }
    }
}
