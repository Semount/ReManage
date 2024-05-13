using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("dishes")]
    public class DishModel : ViewModelBase
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("weight")]
        public int Weight { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("recipe")]
        public string Recipe { get; set; }

        [Column("cooking_time")]
        public TimeSpan? CookingTime { get; set; }

        [Column("image")]
        public byte[] Image { get; set; }

        [NotMapped]
        public BitmapImage ImageSource
        {
            get
            {
                if (Image == null || Image.Length == 0)
                {
                    return null;
                }

                using (MemoryStream ms = new MemoryStream(Image))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            }
        }
    }
}