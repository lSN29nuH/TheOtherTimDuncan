﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOTD.Mvc.FluentHtml.Elements;

namespace TOTD.Test.FluentHtml
{
    [TestClass]
    public class InputCheckboxElementTests : BaseFormElementTests
    {
        [TestMethod]
        public void SuccessfullySetsInputType()
        {
            new InputCheckboxElement(GetHtmlHelper())
                .ToHtmlString()
                .Should()
                .Be(@"<input type=""checkbox"" />");
        }
    }
}