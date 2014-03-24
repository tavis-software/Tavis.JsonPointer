using System;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tavis
{
    public class JsonPointerTests
    {

        public JToken GetSample1()
        {
            var jobject = new JObject();
            jobject["foo"] = new JArray(new []{"bar","baz"});
            jobject[""] = 0;
            jobject["a/b"] = 1;
            jobject["c%d"] = 2;
            jobject["e^f"] = 3;
            jobject["g|h"] = 4;
            jobject["i\\j"] = 5;
            jobject["k\"l"] = 6;
            jobject[" "] = 7;
            jobject["m~n"] = 8;
            jobject["tee"] = new JObject(new []
            {
                new JProperty("orange","a1"),
                new JProperty("blue","a2"),
                new JProperty("black","a3"),
                new JProperty("pink",new JArray(new []
                        {
                            new JValue("orange"),
                            new JValue("blue")
                        }))
            });
            return jobject;
        }

        [Fact]
        public void PointToRoot()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("");

            var result = pointer.Find(sample);

            Assert.Equal(sample, result);
        }

        [Fact]
        public void PointToArray()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo");

            var result = pointer.Find(sample);

            Assert.Equal(sample["foo"], result);
        }

        [Fact]
        public void PointToArrayElement()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo/0");

            var result = pointer.Find(sample);

            Assert.Equal(sample["foo"][0], result);
        }

        [Fact]
        public void New_item_reference()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo/-");

            Assert.True(pointer.IsNewPointer());
        }

        [Fact]
        public void Not_New_item_reference()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo/0");

            Assert.False(pointer.IsNewPointer());
        }

        [Fact]
        public void Parent_pointer()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo/-");

            Assert.Equal("/foo", pointer.ParentPointer.ToString());
        }

        [Fact]
        public void Point_to_array_element_that_does_not_exist()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/foo/10");
            var thrown = false;
            JToken result;
            try
            {
                result = pointer.Find(sample);
            }
            catch (ArgumentException)
            {

                thrown = true;
            }

            Assert.True(thrown);
        }
        [Fact]
        public void PointToPropertyWithEmptyKey()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/");

            var result = pointer.Find(sample);

            Assert.Equal(sample[""], result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedSlash()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/a~1b");

            var result = pointer.Find(sample);

            Assert.Equal(1, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedPercent()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/c%d");

            var result = pointer.Find(sample);

            Assert.Equal(2, (int)result);
        }

        [Fact]
        public void PointToTokenWithEncodedPercent()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/c%25d");

            var result = pointer.Find(sample);

            Assert.Equal(2, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedCaret()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/e^f");

            var result = pointer.Find(sample);

            Assert.Equal(3, (int)result);
        }

        [Fact]
        public void PointToTokenWithEncodedCaret()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/e%5Ef");

            var result = pointer.Find(sample);

            Assert.Equal(3, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedPipe()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/g|h");

            var result = pointer.Find(sample);

            Assert.Equal(4, (int)result);
        }
        [Fact]
        public void PointToTokenWithEscapedPipe()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/g%7Ch");

            var result = pointer.Find(sample);

            Assert.Equal(4, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedBackSlash()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/i\\j");

            var result = pointer.Find(sample);

            Assert.Equal(5, (int)result);
        }

        
        [Fact]
        public void PointToTokenWithEscapedBackSlash()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/i%5Cj");

            var result = pointer.Find(sample);

            Assert.Equal(5, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedQuote()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/k\"l");

            var result = pointer.Find(sample);

            Assert.Equal(6, (int)result);
        }

        [Fact]
        public void PointToTokenWithEscapedQuote()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/k%22l");

            var result = pointer.Find(sample);

            Assert.Equal(6, (int)result);
        }

        [Fact]
        public void PointToTokenWithSpace()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/ ");

            var result = pointer.Find(sample);

            Assert.Equal(7, (int)result);
        }


        [Fact]
        public void PointToTokenWithEscapedSpace()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/%20");

            var result = pointer.Find(sample);

            Assert.Equal(7, (int)result);
        }

        [Fact]
        public void PointToTokenWithEmbeddedTilde()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/m~0n");

            var result = pointer.Find(sample);

            Assert.Equal(8, (int)result);
        }

        [Fact]
        public void PointToTokenTwoLevelsDeep()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/tee/black");

            var result = pointer.Find(sample);

            Assert.Equal("a3", (string)result);
        }
        [Fact]
        public void PointToTokenThreeLevelsDeep()
        {
            var sample = GetSample1();

            var pointer = new JsonPointer("/tee/pink/1");

            var result = pointer.Find(sample);

            Assert.Equal("blue", (string)result);
        }

        [Fact]
        public void Sample2Author()
        {
            var sample = GetSample2();

            var pointer = new JsonPointer("/books/1/author");

            var result = pointer.Find(sample);

            Assert.Equal("John Steinbeck", (string)result);
        }


        public JToken GetSample2()
        {
            return JToken.Parse(@"{
    'books': [
        {
          'title' : 'The Great Gatsby',
          'author' : 'F. Scott Fitzgerald'
        },
        {
          'title' : 'The Grapes of Wrath',
          'author' : 'John Steinbeck'
        }
    ]
}");
        }
    }
}
