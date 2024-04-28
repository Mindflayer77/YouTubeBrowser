using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


namespace YoutubeBrowser.Models
{
    /// <summary>
    /// Database model representing Playlist table
    /// </summary>
    public class Playlist
    {
        /// <summary>
        /// Id of the playlist | 
        /// Primary key | 
        /// Autoincremented
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Name of the playlist | 
        /// Required parameter
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of the playlist | 
        /// Optional parameter
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Collection of Videos which are included in the playlist
        /// </summary>
        public ICollection<Video> Videos { get; set; }
    }
}
