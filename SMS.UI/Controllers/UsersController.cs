
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _service.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(long id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var userId = await _service.CreateAsync(dto);

                if (userId <= 0)
                {
                    ModelState.AddModelError(
                        string.Empty, "Unable to create the user.");

                    return View(dto);
                }

                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the user. Check that the email is unique and the selected tenant exists.");

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, UserDto dto)
        {
            if (id != dto.UserId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var updated = await _service.UpdateAsync(dto);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the user. Check that the email is unique and the selected tenant exists.");

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                {
                    TempData["ErrorMessage"] =
                        "User was not found or is already inactive.";
                }
                else
                {
                    TempData["SuccessMessage"] =
                        "User deactivated successfully.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "Unable to deactivate the user because it may be referenced by other records.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}