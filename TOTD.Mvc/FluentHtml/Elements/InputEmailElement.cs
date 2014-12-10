﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using TOTD.Mvc.FluentHtml.Html;

namespace TOTD.Mvc.FluentHtml.Elements
{
    public class InputEmailElement : BaseInputElement<InputEmailElement>
    {
        public InputEmailElement(HtmlHelper htmlHelper)
            : base(HtmlInputType.Email, htmlHelper)
        {
        }
    }

    public class InputEmailElement<TModel> : InputEmailElement
    {
        public InputEmailElement(HtmlHelper<TModel> htmlHelper)
            : base(htmlHelper)
        {
        }

        public InputEmailElement For<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            SetAttributesFromModelProperty(expression);
            return this;
        }
    }
}
