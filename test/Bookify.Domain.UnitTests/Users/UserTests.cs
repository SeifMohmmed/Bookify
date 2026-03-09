using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;
public class UserTests : BaseTest
{

    /*
       Naming Convention:
       ==================
       MethodName_ShouldExpectedBehavior_WhenCondition

       Arrange ==> Prepare the objects and data required for the test.
       Act ==> Execute the method being tested.
       Assert ==> Verify the expected result.
    */

    [Fact]
    public void Create_Should_SetPropertyuValues()
    {
        //Act 
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        //Assert
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
    }

    [Fact]
    public void Create_Should_Raise_UserCreatedDomainEvent()
    {
        //Act 
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        //Assert
        var domainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);

        domainEvent.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Create_Should_AddRegisteredRoleToUser()
    {
        //Act 
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        //Assert
        user.Roles.Should().Contain(Role.Registered);
    }
}
