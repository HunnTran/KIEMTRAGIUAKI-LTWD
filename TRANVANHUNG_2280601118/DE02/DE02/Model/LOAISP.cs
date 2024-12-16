namespace DE02.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LOAISP")]
    public partial class LOAISP
    {
        [Key]
        [StringLength(2)]
        public string MALOAI { get; set; }

        [Required]
        [StringLength(20)]
        public string TENLOAI { get; set; }
    }
}
