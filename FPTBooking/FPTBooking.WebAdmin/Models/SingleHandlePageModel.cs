﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.WebAdmin.Models
{
    public abstract class SingleHandlePageModel<T> : BasePageModel<T>
    {
        protected abstract IActionResult OnAllMethod();

        public IActionResult OnGet()
        {
            return OnAllMethod();
        }
        public IActionResult OnPost()
        {
            return OnAllMethod();
        }
        public IActionResult OnPatch()
        {
            return OnAllMethod();
        }
        public Task<IActionResult> OnPatchAsync()
        {
            return Task.FromResult(OnAllMethod());
        }
        public IActionResult OnDelete()
        {
            return OnAllMethod();
        }
        public IActionResult OnPut()
        {
            return OnAllMethod();
        }
    }
}
