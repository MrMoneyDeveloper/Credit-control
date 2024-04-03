using System;
using System.ComponentModel.DataAnnotations;

namespace eBaseApp.Models
{
    interface IAuditable
    {
        int Id { get; set; }

        int? CreatedBySystemUserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        DateTime? CreatedDateTime { get; set; }

        int? ModifiedBySystemUserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        DateTime? ModifiedDateTime { get; set; }
    }
}