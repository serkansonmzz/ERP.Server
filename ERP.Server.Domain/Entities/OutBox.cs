using System;
using ERP.Server.Domain.Entities.Common;

namespace ERP.Server.Domain.Entities;

public sealed class OutBox : Entity, ISoftDelete
{
   public TableNameEnum TableName { get; set; }

   public Guid RecordId { get; set; }

   public OperationEnum Operation { get; set; }

   public bool IsCompleted { get; set; }

   public int TryCount { get; set;}
}

public enum TableNameEnum
{
    Product = 1 
}


public enum OperationEnum
{
    Insert = 1,
    Update = 2,
    Delete = 3
}
