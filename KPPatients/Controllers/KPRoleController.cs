using KPPatients.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPPatients.Controllers
{
    public class KPRoleController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public KPRoleController(UserManager<IdentityUser> userManger, RoleManager<IdentityRole> roleManger)
        {
            this.userManager = userManger;
            this.roleManager = roleManger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        public IActionResult RoleList()
        {
            var roleList = roleManager.Roles;
            ViewBag.RoleList = roleList;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRole model)
        {

            if (ModelState.IsValid)
            {
                var isRole = roleManager.FindByNameAsync(model.RoleName);

                if (isRole.Result != null)
                {
                    TempData["message"] = "Role is aleady exists.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        IdentityRole role = new IdentityRole { Name = model.RoleName };

                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            TempData["message"] = "Role added: " + model.RoleName;
                            return RedirectToAction(nameof(RoleList));
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = ex.GetBaseException().Message.ToString();
                        ModelState.AddModelError("", ex.GetBaseException().Message.ToString());
                    }

                }
            }
            return RedirectToAction(nameof(RoleList));

        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["message"] = "Role is not exists.";
            }

            var model = new EditRole
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditRole model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                TempData["message"] = "Role is not exists.";
            }
            else
            {
                try
                {

                    role.Name = model.RoleName;

                    TempData["message"] = "Role deleted: " + role.Name;
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleList");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.GetBaseException().Message.ToString();
                    ModelState.AddModelError("", ex.GetBaseException().Message.ToString());
                }
            }

            return View(model);
        }

        public async Task<IActionResult> UserRole(string RoleId)
        {
            var role = await roleManager.FindByIdAsync(RoleId);
            //ViewBag.RoleName = role.Name;
            ViewBag.RoleId = RoleId;

            var model = new List<UserRole>();
            var dropdownModel = new List<UserRole>();

            foreach (var user in userManager.Users)
            {

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    var userrole = new UserRole
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };


                    model.Add(userrole);
                }

                var userroles = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                dropdownModel.Add(userroles);

            }


            ViewData["Users"] = new SelectList(dropdownModel.OrderBy(x => x.UserName), "UserId", "UserName");
            ;
            ViewBag.UsersRole = model;



            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(UserRole model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                // Error meesage
                ViewBag.ErrorMessage = $"Role not found";
                TempData["message"] = $"Role not found";
            }

            try
            {
                var userDetail = await userManager.FindByIdAsync(model.UserId);
                IdentityResult result = null;
                if (!(await userManager.IsInRoleAsync(userDetail, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(userDetail, role.Name);
                    TempData["message"] = "User added in role: " + role.Name;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.GetBaseException().Message.ToString();
                ModelState.AddModelError("", ex.GetBaseException().Message.ToString());
            }
            return RedirectToAction("UserRole", new { RoleId = model.RoleId });
        }

        public async Task<IActionResult> RemoveUserRole(string RoleId, string UserId)
        {
            var role = await roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                // Error meesage
                ViewBag.ErrorMessage = $"Role not found";
                TempData["message"] = $"Role not found";
            }
            else
            {
                try
                {
                    var userDetail = await userManager.FindByIdAsync(UserId);
                    IdentityResult result = null;

                    if (await userManager.IsInRoleAsync(userDetail, role.Name))
                    {
                        result = await userManager.RemoveFromRoleAsync(userDetail, role.Name);
                        TempData["message"] = "User removed from role: " + role.Name;
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.GetBaseException().Message.ToString();
                    ModelState.AddModelError("", ex.GetBaseException().Message.ToString());
                }
            }

            return RedirectToAction("UserRole", new { RoleId = RoleId });
        }

    }
}
