using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;
using WishApp.Data.Utils;

namespace WishApp.Data.Models.Master;

public class SubWish
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    [ForeignKey("Wish")]
    public required string WishId { get; set; }
    [JsonIgnore]
    public Wish? Wish { get; set; }

    public AppEntityState EntityState { get; set; }
}
