using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YoutubeBrowser.Models
{
    public class PlaylistVideo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }



        [ForeignKey("VideoId")]
        public virtual Video Video { get; set; }

        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; }
        //public DbPlaylistDbVideo() { }

    }
}