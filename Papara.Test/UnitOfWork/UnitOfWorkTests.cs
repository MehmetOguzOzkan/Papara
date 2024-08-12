using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Papara.Data.Context;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.UnitOfWork
{
    public class UnitOfWorkTests
    {
        private readonly Mock<PaparaDbContext> _mockContext;
        private readonly Papara.Data.UnitOfWork.UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            _mockContext = new Mock<PaparaDbContext>(new DbContextOptions<PaparaDbContext>());
            _unitOfWork = new Papara.Data.UnitOfWork.UnitOfWork(_mockContext.Object);
        }

        [Fact]
        public async Task Complete_Calls_SaveChangesAsync()
        {
            await _unitOfWork.Complete();
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Complete_Throws_Exception()
        {
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _unitOfWork.Complete());
        }

        [Fact]
        public async Task CompleteWithTransaction_Commits_Transaction()
        {
            await _unitOfWork.CompleteWithTransaction();
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompleteWithTransaction_Rollbacks_Transaction_On_Exception()
        {
            var transactionMock = new Mock<IDbContextTransaction>();
            _mockContext.Setup(m => m.Database.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _unitOfWork.CompleteWithTransaction());

            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void Dispose_Calls_Dispose_On_Context()
        {
            _unitOfWork.Dispose();
            _mockContext.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
