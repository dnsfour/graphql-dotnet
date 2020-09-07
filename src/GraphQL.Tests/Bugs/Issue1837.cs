using System;
using System.Collections.Generic;
using GraphQL.Types;
using Shouldly;
using Xunit;

namespace GraphQL.Tests.Bugs
{
    // https://github.com/graphql-dotnet/graphql-dotnet/issues/1837
    public class Issue1837 : QueryTestBase<Issue1837Schema>
    {
        [Fact]
        public void Issue1837_Should_Work()
        {
            var guidStr = Guid.NewGuid().ToString();
            var query = @"
query {
  getsome(input: { valueProp: 1, valueProp2: 2, ints: [3,4], ints2: [5,6], intsList: [7,8], intsList2: [9,10], idProp: """ + guidStr + @""" })
}";
            var expected = @"{
  ""getsome"": null
}";
            AssertQuerySuccess(query, expected, root: guidStr);
        }
    }

    public class Issue1837Schema : Schema
    {
        public Issue1837Schema()
        {
            Query = new Issue1837Query();
        }
    }

    public class Issue1837Query : ObjectGraphType
    {
        public Issue1837Query()
        {
            Field<ListGraphType<IntGraphType>>(
                "getsome",
                arguments: new QueryArguments(
                    new QueryArgument<Issue1837ArrayInputType> { Name = "input" }
                ),
                resolve: ctx =>
                {
                    var arg = ctx.GetArgument<Issue1837ArrayInput>("input");

                    arg.Ints.ShouldNotBeNull();
                    arg.Ints.Length.ShouldBe(2);
                    arg.Ints2.ShouldNotBeNull();
                    arg.Ints2.Length.ShouldBe(2);

                    arg.IntsList.ShouldNotBeNull();
                    arg.IntsList.Count.ShouldBe(2);
                    arg.IntsList2.ShouldNotBeNull();
                    arg.IntsList2.Count.ShouldBe(2);

                    arg.ValueProp.ShouldBe(1);
                    arg.ValueProp2.ShouldBe(2);

                    arg.IdProp.ToString().ShouldBe(ctx.RootValue);

                    return null;
                });
        }
    }

    public class Issue1837ArrayInput
    {
        public int[] Ints { get; set; }

        public int[] Ints2 { get; set; }

        public List<int> IntsList { get; set; }

        public List<int> IntsList2 { get; set; }
        
        public int? ValueProp { get; set; }

        public int ValueProp2 { get; set; }
        public Guid IdProp { get; set; }
    }

    public class Issue1837ArrayInputType : InputObjectGraphType
    {
        public Issue1837ArrayInputType()
        {
            Field<ListGraphType<IntGraphType>>("ints");
            Field<ListGraphType<NonNullGraphType<IntGraphType>>>("ints2");
            Field<ListGraphType<IntGraphType>>("intsList");
            Field<ListGraphType<NonNullGraphType<IntGraphType>>>("intsList2");
            Field<IntGraphType>("valueProp");
            Field<NonNullGraphType<IntGraphType>>("valueProp2");
            Field<NonNullGraphType<IdGraphType>>("IdProp");
        }
    }
}
