using System.ComponentModel.DataAnnotations;

namespace messageSwitch_UI.Models
{
    public class DriverLicenseQueryModel
    {
        [Required(ErrorMessage = "ORI is required")]
        public string ORI { get; set; } = "F1000000";

        [Required(ErrorMessage = "Destination State is required")]
        public string DST { get; set; } = "PA";

        public string? OCF { get; set; }

        public string? OLN { get; set; } = "GHJY88888877777777";

        public string? NAM { get; set; }

        public string? DOB { get; set; }

        public string? SEX { get; set; }

        public string OLS { get; set; } = "CA";

        public bool IMQ { get; set; } = true;
    }
}
