﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.ViewModels;

namespace Thinktecture.IdentityServer.Core.Results
{
    internal class LoggedOutActionResult : HtmlStreamActionResult
    {
        public LoggedOutActionResult(IViewService viewSvc, LoggedOutViewModel model)
            : base(async () => await viewSvc.LoggedOut(model))
        {
            if (viewSvc == null) throw new ArgumentNullException("viewSvc");
            if (model == null) throw new ArgumentNullException("model");
        }
    }
}
