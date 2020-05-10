using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace JsonToClassInit.Test
{
    public class Tests
    {
        private Sample _data;

        [SetUp]
        public void Setup()
        {
            _data = new Sample
                        {
                            Name = "JamisLiao",
                            Age = 36,
                            Weight = 101.32m,
                            IsAdult = true,
                            NickNames = new List<string>
                                            {
                                                "Test",
                                                "123"
                                            },
                            Detail = new List<SampleDetail>
                                         {
                                             new SampleDetail
                                                 {
                                                     Address = "Test1Address",
                                                     Email = "Jamis1@gmail.com"
                                                 },
                                             new SampleDetail
                                                 {
                                                     Address = "Test2Address",
                                                     Email = "Jamis2@gmail.com"
                                                 }
                                         }
                        };
        }

        [Test]
        public void PropertyInfo_Is_String_Non_Collection()
        {
            var propertyInfo = _data.GetType().GetProperty("Name");
            var actual = Program.GeneratorInitStringForString(_data, propertyInfo, false);
            var expected = "Name = \"JamisLiao\",";
            actual.Should().Be(expected);
        }

        [Test]
        public void PropertyInfo_Is_String_Is_Collection()
        {
            var propertyInfo = _data.GetType().GetProperty("NickNames");
            var actual = Program.GeneratorInitStringForString(_data, propertyInfo, true);
            var expected = "new List<string>{ \"Test\", \"123\" },";
            actual.Should().Be(expected);
        }

        [Test]
        public void PropertyInfo_Is_Class_Is_Collection()
        {
            var propertyInfo = _data.GetType().GetProperty("Detail");
            var actual = Program.GeneratorInitStringForString(_data, propertyInfo, true);
            var expected = "new List<SampleDetail>{\"Address\":\"TestAddress\",\"Email\":\"jamisliao@gmail.com\"},{\"Address\":\"Test2Address\",\"Email\":\"jamisliao2@gmail.com\"}},";
            actual.Should().Be(expected);
        }
    }
}