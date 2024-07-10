using Domain.Model.ExternalCommunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace LavoroAgile.Core
{
    /// <summary>
    /// Test di serializzazione
    /// </summary>
    public class SerializerTest
    {
        [Fact]
        public void TestDeserialization()
        {
            var wdCollection = new WorkingDaysTransmission("test");
            wdCollection.WorkingDays = "12/12/2021,22/12/2020".Split(',').Select(d => d).ToList();

            // System.Text.Json non traduce male la lista custom (non serializza Value)
            //var wdCollection = new CustomList();
            //wdCollection.Value = "ciao";
            //wdCollection.Add(new ListElement { MyProperty = 1 });

            JsonSerializerOptions _jsonSerializerOptions = new() { IncludeFields = true };
            var jsonBytes =  JsonSerializer.SerializeToUtf8Bytes(wdCollection, _jsonSerializerOptions);
            var converted = Convert.ToBase64String(jsonBytes);
            var obj = JsonSerializer.Deserialize(jsonBytes, typeof(WorkingDaysTransmission), _jsonSerializerOptions) as WorkingDaysTransmission;

            Assert.Equal("test", obj.UserId);
            Assert.True(obj.WorkingDays.Count == 2);
            
        }

        public class CustomList : List<ListElement>
        {
            public string Value { get; set; }
        }
        public class ListElement
        {
            public int MyProperty { get; set; }
        }

    }
}
