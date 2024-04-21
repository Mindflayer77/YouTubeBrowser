using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace YoutubeBrowser.Models
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string YoutubeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Thumbnail_url { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }

    }
}
