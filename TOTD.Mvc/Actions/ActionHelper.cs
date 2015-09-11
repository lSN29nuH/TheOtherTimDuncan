﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using TOTD.Mvc.FluentHtml;

namespace TOTD.Mvc.Actions
{
    public static class ActionHelper
    {
        private const string controllerSuffix = "Controller";

        private static ConcurrentDictionary<Type, ControllerContext> cache = new ConcurrentDictionary<Type, ControllerContext>();

        /// <summary>
        /// Returns the controller name for use with routing by removing Controller from the end
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns>The controller name with Controller removed from the end, or the full controller name if it does not end with Controller</returns>
        public static string GetControllerRouteName(string controllerName)
        {
            if (controllerName.EndsWith(controllerSuffix))
            {
                return controllerName.Substring(0, controllerName.Length - controllerSuffix.Length);
            }
            else
            {
                return controllerName;
            }
        }

        public static ActionHelperResult GetRouteValues<T>(Expression<Func<T, ActionResult>> actionSelector) where T : IController
        {
            return GetRouteValues<T>(actionSelector, null);
        }

        public static ActionHelperResult GetRouteValues<T>(Expression<Func<T, ActionResult>> actionSelector, object routeValues) where T : IController
        {
            RouteValueDictionary dictionaryValues = new RouteValueDictionary(routeValues);
            return GetRouteValues<T>(actionSelector, dictionaryValues);
        }

        public static ActionHelperResult GetRouteValues<T>(Expression<Func<T, Task<ActionResult>>> actionSelector) where T : IController
        {
            return GetRouteValues<T>((actionSelector.Body as MethodCallExpression), null);
        }

        public static ActionHelperResult GetRouteValues<T>(Expression<Func<T, Task<ActionResult>>> actionSelector, object routeValues) where T : IController
        {
            RouteValueDictionary dictionaryValues = new RouteValueDictionary(routeValues);
            return GetRouteValues<T>((actionSelector.Body as MethodCallExpression), dictionaryValues);
        }

        public static ActionHelperResult GetRouteValues<T>(Expression<Func<T, ActionResult>> actionSelector, RouteValueDictionary routeValues) where T : IController
        {
            return GetRouteValues<T>((actionSelector.Body as MethodCallExpression), routeValues);
        }

        public static ActionHelperResult GetRouteValues<T>(MethodCallExpression methodExpression, RouteValueDictionary routeValues) where T : IController
        {
            ActionHelperResult result = new ActionHelperResult()
            {
                RouteValues = routeValues ?? new RouteValueDictionary()
            };

            Type controllerType = typeof(T);

            if (methodExpression == null || methodExpression.Object.Type != controllerType)
            {
                throw new ArgumentException("You must call a method of " + controllerType.Name, "actionSelector");
            }

            string areaName = string.Empty;  // Default area for non-area based actions is an empty string

            // Check for cached controller context
            ControllerContext controllerContext;
            if (cache.TryGetValue(controllerType, out controllerContext))
            {
                // Use cached values
                result.ControllerName = controllerContext.ControllerName;
                areaName = controllerContext.AreaName;
            }
            else
            {
                result.ControllerName = GetControllerRouteName(controllerType.Name);

                // Get the area from the controller if it has the attribute
                RouteAreaAttribute controllerArea = controllerType.GetCustomAttribute<RouteAreaAttribute>();
                if (controllerArea != null)
                {
                    areaName = controllerArea.AreaName;
                }

                // Cache the results
                controllerContext = new ControllerContext(result.ControllerName, areaName);
                cache.TryAdd(controllerType, controllerContext);
            }

            // Only set the area if it is not already there
            if (!result.RouteValues.ContainsKey(RouteValueKeys.Area))
            {
                result.RouteValues.Add(RouteValueKeys.Area, areaName);
            }

            // Action name is the name of the method being called
            result.ActionName = methodExpression.Method.Name;

            // Check for cached action 
            string[] parameterNames;
            if (!controllerContext.TryGetActionParameterNames(result.ActionName, out parameterNames))
            {
                parameterNames = methodExpression.Method.GetParameters().Select(x => x.Name).ToArray();

                // Cache parameter names
                controllerContext.AddAction(result.ActionName, parameterNames);
            }

            int i = 0;
            foreach (Expression arg in methodExpression.Arguments)
            {
                string parameterName = parameterNames[i];
                if (!result.RouteValues.ContainsKey(parameterName))
                {
                    object parameterValue;
                    if (arg.NodeType == ExpressionType.Constant)
                    {
                        parameterValue = ((ConstantExpression)arg).Value;
                    }
                    else
                    {
                        parameterValue = Expression.Lambda(arg).Compile().DynamicInvoke(null);
                    }
                    if (parameterValue != null)
                    {
                        Type parameterType = parameterValue.GetType();
                        if (parameterType.IsClass && parameterType != typeof(string))
                        {
                            RouteValueDictionary modelValues = new RouteValueDictionary(parameterValue);
                            foreach (KeyValuePair<string, object> routeValue in modelValues)
                            {
                                result.RouteValues.Add(routeValue.Key, routeValue.Value);
                            }
                        }
                        else
                        {
                            result.RouteValues.Add(parameterName, parameterValue);
                        }
                    }
                }
                i++;
            }

            return result;
        }

        private class ControllerContext
        {
            private ConcurrentDictionary<string, string[]> _actions;

            public ControllerContext(string controllerName, string areaName)
            {
                this.ControllerName = controllerName;
                this.AreaName = areaName;

                this._actions = new ConcurrentDictionary<string, string[]>();
            }

            public string ControllerName
            {
                get;
                private set;
            }

            public string AreaName
            {
                get;
                private set;
            }

            public bool TryGetActionParameterNames(string actionName, out string[] parameterNames)
            {
                return _actions.TryGetValue(actionName, out parameterNames);
            }

            public void AddAction(string actionName, string[] parameterNames)
            {
                _actions.TryAdd(actionName, parameterNames);
            }
        }
    }

    public class ActionHelperResult
    {
        public string ControllerName
        {
            get;
            set;
        }

        public string ActionName
        {
            get;
            set;
        }

        public RouteValueDictionary RouteValues
        {
            get;
            set;
        }
    }
}