﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures
{
    public class ListItem
    {
        public string Icon { get; set; } = "info.png";
        public string Title { get; set; } = "List Item";
        public string Description { get; set; } = "A list ItemData with sample data.";
        public Page? NavigationPage { get; set; }
    }
}
