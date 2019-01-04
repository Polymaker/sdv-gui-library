using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Polymaker.SdvUI
{
    public enum ContentAlignment
    {
        /// <summary>Le contenu est aligné verticalement en haut et horizontalement sur le côté gauche.</summary>
        TopLeft = 1,
        /// <summary>Le contenu est aligné verticalement en haut et horizontalement au centre.</summary>
        TopCenter,
        /// <summary>Le contenu est aligné verticalement en haut et horizontalement sur le côté droit.</summary>
        TopRight = 4,
        /// <summary>Le contenu est aligné verticalement au milieu et horizontalement sur le côté gauche.</summary>
        MiddleLeft = 0x10,
        /// <summary>Le contenu est aligné verticalement au milieu et horizontalement au centre.</summary>
        MiddleCenter = 0x20,
        /// <summary>Le contenu est aligné verticalement au milieu et horizontalement sur le côté droit.</summary>
        MiddleRight = 0x40,
        /// <summary>Le contenu est aligné verticalement en bas et horizontalement sur le côté gauche.</summary>
        BottomLeft = 0x100,
        /// <summary>Le contenu est aligné verticalement en bas et horizontalement au centre.</summary>
        BottomCenter = 0x200,
        /// <summary>Le contenu est aligné verticalement en bas et horizontalement sur le côté droit.</summary>
        BottomRight = 0x400
    }

}
