using Microsoft.EntityFrameworkCore;
using Moq;
using Papara.Data.Context;
using Papara.Data.Entities;
using Papara.Data.Repositories;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

public class TestEntity : BaseEntity
{
    public string Name { get; set; }
}

namespace Papara.Test.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly Mock<DbSet<TestEntity>> _mockSet;
        private readonly Mock<PaparaDbContext> _mockContext;
        private readonly GenericRepository<TestEntity> _repository;

        public GenericRepositoryTests()
        {
            _mockSet = new Mock<DbSet<TestEntity>>();
            _mockContext = new Mock<PaparaDbContext>();
            _mockContext.Setup(m => m.Set<TestEntity>()).Returns(_mockSet.Object);
            _repository = new GenericRepository<TestEntity>(_mockContext.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id, IsActive = true };
            _mockSet.Setup(m => m.FindAsync(id)).ReturnsAsync(entity);

            // Act
            var result = await _repository.GetById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEntities_WhenCalled()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { IsActive = true },
                new TestEntity { IsActive = true }
            };
            var queryable = entities.AsQueryable();
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.Equal(entities.Count, result.Count());
        }

        [Fact]
        public async Task Insert_ShouldAddEntity_WhenCalled()
        {
            // Arrange
            var entity = new TestEntity { IsActive = true };

            // Act
            var result = await _repository.Insert(entity);

            // Assert
            _mockSet.Verify(m => m.AddAsync(entity, default), Times.Once);
            Assert.Equal(entity, result);
        }

        [Fact]
        public void Update_ShouldModifyEntity_WhenCalled()
        {
            // Arrange
            var entity = new TestEntity { IsActive = true };

            // Act
            _repository.Update(entity);

            // Assert
            _mockSet.Verify(m => m.Update(entity), Times.Once);
        }

        [Fact]
        public void Delete_ShouldRemoveEntity_WhenCalled()
        {
            // Arrange
            var entity = new TestEntity { IsActive = true };

            // Act
            _repository.Delete(entity);

            // Assert
            _mockSet.Verify(m => m.Remove(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveEntity_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id, IsActive = true };
            _mockSet.Setup(m => m.FindAsync(id)).ReturnsAsync(entity);

            // Act
            await _repository.Delete(id);

            // Assert
            _mockSet.Verify(m => m.Remove(entity), Times.Once);
        }

        [Fact]
        public async Task SoftDelete_ShouldMarkEntityAsInactive_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id, IsActive = true };
            _mockSet.Setup(m => m.FindAsync(id)).ReturnsAsync(entity);

            // Act
            await _repository.SoftDelete(id);

            // Assert
            Assert.False(entity.IsActive);
            _mockSet.Verify(m => m.Update(entity), Times.Once);
        }

        [Fact]
        public async Task Where_ShouldReturnFilteredEntities_WhenPredicateIsApplied()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> predicate = e => e.IsActive;
            var entities = new List<TestEntity>
            {
                new TestEntity { IsActive = true },
                new TestEntity { IsActive = false }
            };
            var queryable = entities.AsQueryable();
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Act
            var result = await _repository.Where(predicate);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task FirstOrDefault_ShouldReturnEntity_WhenPredicateMatches()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> predicate = e => e.IsActive;
            var entity = new TestEntity { IsActive = true };
            var queryable = new List<TestEntity> { entity }.AsQueryable();
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Act
            var result = await _repository.FirstOrDefault(predicate);

            // Assert
            Assert.Equal(entity, result);
        }
    }
}
