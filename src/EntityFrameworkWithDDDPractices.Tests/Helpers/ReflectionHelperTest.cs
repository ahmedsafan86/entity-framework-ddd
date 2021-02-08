using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace EntityFrameworkWithDDDPractices.Tests.Helpers
{
    public class ReflectionHelperTest
    {
        [Theory]
        [InlineData(typeof(GenericClass<>), false)]
        [InlineData(typeof(AbstractClass), false)]
        [InlineData(typeof(IFace), false)]
        [InlineData(typeof(Implementation), true)]
        [InlineData(typeof(GenericChild), true)]
        public void IsConcrete_WorksAsExpected(Type type, bool expectedResult)
        {
            type.IsConcrete().ShouldBe(expectedResult, type.Name);
        }

        [Fact]
        public void GetAllConcreteTypesImplementingGenericInterface_WorksAsExpected()
        {
            // Arrange
            var expectedResult = new[]
            {
                (typeof(ImplementingGenericInterface1), typeof(string)),
                (typeof(ImplementingGenericInterface2), typeof(int)),
                (typeof(ImplementingGenericInterface3), typeof(DayOfWeek))
            };

            // Act
            var result = ReflectionHelper.GetAllConcreteTypesImplementingGenericInterface(typeof(IGenericInterface<>));

            // Assert
            result.Count().ShouldBe(expectedResult.Count());
            Enumerable.SequenceEqual(expectedResult, result).ShouldBeTrue();
        }
    }

    class GenericClass<T> { }

    abstract class AbstractClass { }

    interface IFace { }

    sealed class Implementation : IFace { }

    class GenericChild : GenericClass<int> { }

    interface IGenericInterface<T> { }

    class ImplementingGenericInterface1 : IGenericInterface<string> { }

    class ImplementingGenericInterface2 : IGenericInterface<int> { }
    class ImplementingGenericInterface3 : IGenericInterface<DayOfWeek> { }
}