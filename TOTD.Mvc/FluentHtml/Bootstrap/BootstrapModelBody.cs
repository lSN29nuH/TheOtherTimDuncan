﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TOTD.Mvc.FluentHtml.Elements;
using TOTD.Mvc.FluentHtml.Html;

namespace TOTD.Mvc.FluentHtml.Bootstrap
{
    public class BootstrapModelBody : BaseContainerElement<BootstrapModelBody>
    {
        public BootstrapModelBody(HtmlHelper htmlHelper)
            : base(HtmlTag.Div, htmlHelper)
        {
            Class("modal-body");
        }
    }
}