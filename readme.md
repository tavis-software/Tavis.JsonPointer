# Tavis.JsonPointer

This library is an implementation of a Json Pointer [RFC 6901](http://tools.ietf.org/html/rfc6901) based on the Newtonsoft JSON.Net library.  


With the following sample,
			
	{
	    "books": [
	        {
	          "title" : "The Great Gatsby",
	          "author" : "F. Scott Fitzgerald"
	        },
	        {
	          "title" : "The Grapes of Wrath",
	          "author" : "John Steinbeck"
	        }
	    ]
	}

The following code can use a Json Pointer to select a fragment of the Json document.

            var pointer = new JsonPointer("/books/1/author");
            var result = pointer.Find(sample);
            Assert.Equal("John Steinbeck", (string)result);