using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler.IEXData.Stock
{
    /*
symbol	string	
companyName	string	
exchange	string	
industry	string	
website	string	
description	string	
CEO	string	
issueType	string	refers to the common issue type of the stock. 
ad – American Depository Receipt (ADR’s) 
re – Real Estate Investment Trust (REIT’s) 
ce – Closed end fund (Stock and Bond Fund) 
si – Secondary Issue 
lp – Limited Partnerships 
cs – Common Stock 
et – Exchange Traded Fund (ETF) 
(blank) = Not Available, i.e., Warrant, Note, or (non-filing) Closed Ended Funds
sector	string	
tags	array	an array of strings used to classify the company. 
     */

    [JsonObject]
    public class Company
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid _id { get; set; }

        public string symbol { get; set; }
        public string companyName { get; set; }
        public string exchange { get; set; }
        public string industry { get; set; }
        public string webSite { get; set; }
        public string description { get; set; }
        public string CEO { get; set; }
        public string issueType { get; set; }
        public string sector { get; set; }
        public string[] tags { get; set; }
    }

    public class SymbolIdGenerator : IIdGenerator
    {

        public object GenerateId(object container, object document)
        {

            return string.Format($"{container.ToString()}_{Guid.NewGuid().ToString()}");
        }

        public bool IsEmpty(object id)
        {
            return id == null || String.IsNullOrEmpty(id.ToString());
        }
    }
}
