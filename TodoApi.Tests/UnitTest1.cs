using TodoApi.Controller;
using TodoApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System;
using System.Collections.Generic;

namespace TodoApi.Tests;

public class TodoControllerTests
{
    private readonly TodoController _todoController;
    private readonly Mock<ITodoRepository> _todoRepositoryMock;

    public TodoControllerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _todoController = new TodoController(_todoRepositoryMock.Object);
    }

    [Fact]
    public void GetAll_ReturnsTodoItems()
    {
        // arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 1", Description = "Test Description 1" },
            new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 2", Description = "Test Description 2" }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        var result = _todoController.GetAll();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Equal(todoItems.Count, returnedTodos.Count);
        _todoRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void Get_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        _todoRepositoryMock.Setup(repo => repo.Get(invalidId)).Returns((TodoItem?)null);
        
        var result = _todoController.Get(invalidId);
        Assert.IsType<NotFoundResult>(result);
        _todoRepositoryMock.Verify(repo => repo.Get(invalidId), Times.Once);
    }

    [Fact]
    public void Create_WithValidTodoItem_ReturnsCreatedAtAction()
    {
        var todoItem = new TodoItem { Title = "Test Todo 1", Description = "Test Description 1" };
        _todoRepositoryMock.Setup(repo => repo.Add(It.IsAny<TodoItem>()));

        var result = _todoController.Create(todoItem);
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(todoItem, createdAtResult.Value);
        _todoRepositoryMock.Verify(repo => repo.Add(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsCreatedAtAction()
    {
        var invalidTodoItem = new TodoItem { Title = "", Description = "Test Description" };
        _todoRepositoryMock.Setup(repo => repo.Add(It.IsAny<TodoItem>()));
        
        var result = _todoController.Create(invalidTodoItem);
        Assert.IsType<CreatedAtActionResult>(result);
        _todoRepositoryMock.Verify(repo => repo.Add(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public void Update_WithValidTodoItem_ReturnsNoContent()
    {
        var todoId = Guid.NewGuid();
        var todoItem = new TodoItem { Id = todoId, Title = "Updated Title", Description = "Updated Description" };
        
        _todoRepositoryMock.Setup(repo => repo.Update(It.IsAny<TodoItem>()));
        var result = _todoController.Update(todoId, todoItem);
        Assert.IsType<NoContentResult>(result);
        _todoRepositoryMock.Verify(repo => repo.Update(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public void Delete_WithValidId_ReturnsNoContent()
    {
        var todoId = Guid.NewGuid();
        _todoRepositoryMock.Setup(repo => repo.Delete(It.IsAny<Guid>()));

        var result = _todoController.Delete(todoId);
        Assert.IsType<NoContentResult>(result);
        _todoRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void Update_WithNonExistentId_ReturnsNotFound()
    {
        var todoId = Guid.NewGuid();
        var todoItem = new TodoItem { Id = todoId, Title = "Updated Title", Description = "Updated Description" };
        
        _todoRepositoryMock.Setup(repo => repo.Update(It.IsAny<TodoItem>()));
        var result = _todoController.Update(todoId, todoItem);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Delete_WithNonExistentId_ReturnsNoContent()
    {

        var todoId = Guid.NewGuid();
        _todoRepositoryMock.Setup(repo => repo.Delete(It.IsAny<Guid>()));

        var result = _todoController.Delete(todoId);
        Assert.IsType<NoContentResult>(result);
    }

      [Fact]
    public void GetAll_ReturnsTodoItems_WithTitleContains()
    {
        // arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 1", Description = "Test Description 1" },
            new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 2", Description = "Test Description 2" }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        var result = _todoController.GetAll(title: "Test Todo 1");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var countInfo = responseData?.GetType().GetProperty("count");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        var returnedCount = countInfo?.GetValue(responseData);
        
        Assert.NotNull(returnedTodos);
        Assert.Equal(1, returnedCount);
        _todoRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void GetAll_WithIsCompletedFilter_ReturnsFilteredItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Completed Todo", IsCompleted = true, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Incomplete Todo", IsCompleted = false, CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(isCompleted: true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var countInfo = responseData?.GetType().GetProperty("count");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        var returnedCount = countInfo?.GetValue(responseData);
        
        Assert.NotNull(returnedTodos);
        Assert.Single(returnedTodos);
        Assert.All(returnedTodos, t => Assert.True(t.IsCompleted));
        Assert.Equal(1, returnedCount);
    }

    [Fact]
    public void GetAll_WithTitleFilter_ReturnsMatchingItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Buy groceries", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Clean house", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Buy books", CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(title: "Buy");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var countInfo = responseData?.GetType().GetProperty("count");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Equal(2, returnedTodos.Count);
        Assert.All(returnedTodos, t => Assert.Contains("Buy", t.Title, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(2, countInfo?.GetValue(responseData));
    }

    [Fact]
    public void GetAll_WithDueDateRangeFilter_ReturnsItemsInRange()
    {
        // Arrange
        var dueDate1 = DateTime.Today.AddDays(1);
        var dueDate2 = DateTime.Today.AddDays(5);
        var dueDate3 = DateTime.Today.AddDays(10);
        
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 1", DueDate = dueDate1, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 2", DueDate = dueDate2, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 3", DueDate = dueDate3, CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(dueFrom: DateTime.Today.AddDays(2), dueTo: DateTime.Today.AddDays(8));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Single(returnedTodos);
        Assert.Equal(dueDate2, returnedTodos[0].DueDate);
    }

    [Fact]
    public void GetAll_WithCreatedDateRangeFilter_ReturnsItemsInRange()
    {
        // Arrange
        var createdDate1 = DateTime.Today.AddDays(-10);
        var createdDate2 = DateTime.Today.AddDays(-5);
        var createdDate3 = DateTime.Today;
        
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Old Todo", CreatedAt = createdDate1 },
            new TodoItem { Id = Guid.NewGuid(), Title = "Recent Todo", CreatedAt = createdDate2 },
            new TodoItem { Id = Guid.NewGuid(), Title = "New Todo", CreatedAt = createdDate3 }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(createdFrom: DateTime.Today.AddDays(-7), createdTo: DateTime.Today.AddDays(-1));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        Assert.NotNull(returnedTodos);
        Assert.Single(returnedTodos);
        Assert.Equal(createdDate2, returnedTodos[0].CreatedAt);
    }

    [Fact]
    public void GetAll_WithMultipleFilters_ReturnsCorrectlyFilteredItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Complete urgent task", IsCompleted = true, DueDate = DateTime.Today.AddDays(1), CreatedAt = DateTime.Today.AddDays(-1) },
            new TodoItem { Id = Guid.NewGuid(), Title = "Incomplete urgent task", IsCompleted = false, DueDate = DateTime.Today.AddDays(1), CreatedAt = DateTime.Today.AddDays(-2) },
            new TodoItem { Id = Guid.NewGuid(), Title = "Complete normal task", IsCompleted = true, DueDate = DateTime.Today.AddDays(5), CreatedAt = DateTime.Today.AddDays(-3) },
            new TodoItem { Id = Guid.NewGuid(), Title = "Incomplete normal task", IsCompleted = false, DueDate = DateTime.Today.AddDays(5), CreatedAt = DateTime.Today.AddDays(-4) }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act - Filter for incomplete tasks due tomorrow
        var result = _todoController.GetAll(
            isCompleted: false,
            dueFrom: DateTime.Today,
            dueTo: DateTime.Today.AddDays(2),
            title: "urgent");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        Assert.NotNull(returnedTodos);
        
        Assert.Single(returnedTodos);
        Assert.False(returnedTodos[0].IsCompleted);
        Assert.Contains("urgent", returnedTodos[0].Title, StringComparison.OrdinalIgnoreCase);
        Assert.True(returnedTodos[0].DueDate >= DateTime.Today && returnedTodos[0].DueDate <= DateTime.Today.AddDays(2));
    }

    [Fact]
    public void GetAll_WithSortByTitleAscending_ReturnsSortedItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Zebra", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Apple", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Monkey", CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(sortBy: "title", sortOrder: "asc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        Assert.NotNull(returnedTodos);
        Assert.Equal("Apple", returnedTodos[0].Title);
        Assert.Equal("Monkey", returnedTodos[1].Title);
        Assert.Equal("Zebra", returnedTodos[2].Title);
    }

    [Fact]
    public void GetAll_WithSortByTitleDescending_ReturnsSortedItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Apple", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Monkey", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Zebra", CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(sortBy: "title", sortOrder: "desc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Equal("Zebra", returnedTodos[0].Title);
        Assert.Equal("Monkey", returnedTodos[1].Title);
        Assert.Equal("Apple", returnedTodos[2].Title);
    }

    [Fact]
    public void GetAll_WithSortByDueDate_ReturnsSortedItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Late", DueDate = DateTime.Today.AddDays(3), CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Early", DueDate = DateTime.Today.AddDays(1), CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Middle", DueDate = DateTime.Today.AddDays(2), CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(sortBy: "dueDate", sortOrder: "asc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Equal("Early", returnedTodos[0].Title);
        Assert.Equal("Middle", returnedTodos[1].Title);
        Assert.Equal("Late", returnedTodos[2].Title);
    }

    [Fact]
    public void GetAll_WithSortByIsCompleted_ReturnsSortedItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Completed 1", IsCompleted = true, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Incomplete 1", IsCompleted = false, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Completed 2", IsCompleted = true, CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Incomplete 2", IsCompleted = false, CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(sortBy: "isCompleted", sortOrder: "asc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        // Incomplete items should come first when ascending (false < true)
        Assert.NotNull(returnedTodos);
        Assert.All(returnedTodos.Take(2), t => Assert.False(t.IsCompleted));
        Assert.All(returnedTodos.Skip(2), t => Assert.True(t.IsCompleted));
    }

    [Fact]
    public void GetAll_WithSortByCreatedAtDescending_ReturnsSortedItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Old", CreatedAt = DateTime.Today.AddDays(-5) },
            new TodoItem { Id = Guid.NewGuid(), Title = "New", CreatedAt = DateTime.Today },
            new TodoItem { Id = Guid.NewGuid(), Title = "Middle", CreatedAt = DateTime.Today.AddDays(-2) }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll(sortBy: "createdAt", sortOrder: "desc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        Assert.NotNull(returnedTodos);
        Assert.Equal("New", returnedTodos[0].Title);
        Assert.Equal("Middle", returnedTodos[1].Title);
        Assert.Equal("Old", returnedTodos[2].Title);
    }

    [Fact]
    public void GetAll_WithInvalidSortField_UsesDefaultSort()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "B", CreatedAt = DateTime.Today.AddDays(-1) },
            new TodoItem { Id = Guid.NewGuid(), Title = "A", CreatedAt = DateTime.Today },
            new TodoItem { Id = Guid.NewGuid(), Title = "C", CreatedAt = DateTime.Today.AddDays(-2) }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act - Use invalid sort field
        var result = _todoController.GetAll(sortBy: "invalidField", sortOrder: "asc");

        // Assert - Should default to createdAt sorting
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        
        // Should be sorted by createdAt ascending (oldest first)
        Assert.NotNull(returnedTodos);
        Assert.Equal("C", returnedTodos[0].Title); // Oldest
        Assert.Equal("B", returnedTodos[1].Title); // Middle
        Assert.Equal("A", returnedTodos[2].Title); // Newest
    }

    [Fact]
    public void GetAll_WithCountInResponse_ReturnsCorrectCount()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 1", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 2", CreatedAt = DateTime.Now },
            new TodoItem { Id = Guid.NewGuid(), Title = "Todo 3", CreatedAt = DateTime.Now }
        };
        _todoRepositoryMock.Setup(repo => repo.GetAll()).Returns(todoItems);

        // Act
        var result = _todoController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseData = okResult.Value;
        var propertyInfo = responseData?.GetType().GetProperty("data");
        var returnedTodos = propertyInfo?.GetValue(responseData) as List<TodoItem>;
        var countInfo = responseData?.GetType().GetProperty("count");
        var returnedCount = countInfo?.GetValue(responseData);
        
        Assert.NotNull(returnedTodos);
        Assert.Equal(3, returnedCount);
    }
}