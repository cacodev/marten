using System.Linq;
using System.Reflection;
using Baseline.Reflection;
using Marten.Testing.Documents;
using Marten.Util;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Marten.Testing.Util
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void get_member_alias_with_one_prop()
        {
            var prop = ReflectionHelper.GetProperty<User>(x => x.FirstName);
            new MemberInfo[] {prop}.ToTableAlias().ShouldBe("first_name");
        }

        [Fact]
        public void get_member_alias_with_two_props()
        {
            var prop1 = ReflectionHelper.GetProperty<UserHolder>(x => x.User);
            var prop2 = ReflectionHelper.GetProperty<User>(x => x.FirstName);
            new MemberInfo[] { prop1, prop2 }.ToTableAlias().ShouldBe("user_first_name");
        }

        public class UserHolder
        {
            public User User { get; set; }
        }

        [Fact]
        public void try_get_json_attribute_property_name_with_jsonnet_attribute()
        {
            var testObject = new JsonPropertyAttributedObject
            {
                PropertyNameDoesntMatter = "you're right, it doesn't matter"
            };

            var result = testObject.GetType().GetProperty(nameof(testObject.PropertyNameDoesntMatter))
                                    .TryGetJsonAttributePropertyName(out var propName);

            result.ShouldBe(true);
            propName.ShouldBe("theObject");
        }

        private class JsonPropertyAttributedObject
        {
            [JsonProperty("theObject")]
            public string PropertyNameDoesntMatter { get; set; }
        }

        [Fact]
        public void try_get_json_attribute_property_name_with_systemjson_attribute()
        {
            var testObject = new JsonPropertyNameAttributedObject
            {
                PropertyNameDoesntMatter = "you're right, it doesn't matter"
            };

            var result = testObject.GetType().GetProperty("PropertyNameDoesntMatter")
                                    .TryGetJsonAttributePropertyName(out var propName);

            result.ShouldBe(true);
            propName.ShouldBe("theObject");
        }

        private class JsonPropertyNameAttributedObject
        {
            [System.Text.Json.Serialization.JsonPropertyName("theObject")]
            public string PropertyNameDoesntMatter { get; set; }
        }

        [Fact]
        public void try_get_json_attribute_property_name_with_no_attribute()
        {
            var testObject = new NoJsonAttributedObject
            {
                PropertyNameDoesntMatter = "you're right, it doesn't matter"
            };

            var result = testObject.PropertyNameDoesntMatter.GetType()
                                    .TryGetJsonAttributePropertyName(out var propName);

            result.ShouldBe(false);
            propName.ShouldBe(null);
        }

        private class NoJsonAttributedObject
        {
            public string PropertyNameDoesntMatter { get; set; }
        }
    }
}
