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
    /// <summary>
    /// Database model representing a Video table
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Id of the Video | 
        /// Primary Key | 
        /// Autoincremented
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// YoutubeId identiying the video | 
        /// Required field
        /// </summary>
        [Required]
        public string YoutubeId { get; set; }

        /// <summary>
        /// Title of the video | 
        /// Required field
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Url of the video's thumbnail | 
        /// Required field
        /// </summary>
        [Required]
        public string Thumbnail_url { get; set; }

        /// <summary>
        /// Collection of playlists that the video belongs to
        /// </summary>
        public virtual ICollection<Playlist> Playlists { get; set; }

    }
}
