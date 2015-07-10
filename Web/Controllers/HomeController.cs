using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using YATDL.Security;

namespace YATDL.Controllers
{
    [Authorize]
    public class HomeController : YATDLControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult New()
        {
            return View();
        }

        public ActionResult Card(int id)
        {
            var model = new TaskModel();

            try
            {
                var dbTask = _unitOfWork.Repository<Task>().Query.FirstOrDefault(f => f.Id == id);
                dbTask.ToModel(model);
            }
            catch (System.Exception ex)
            {
                Logger.Error("OnCard", ex);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(TaskModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var err = "Errors" +
                              string.Join(", ",
                                  ModelState.Values.SelectMany(p => p.Errors)
                                      .Select(p => p.ErrorMessage));
                    Logger.Fatal("OnSave: " + err);

                    throw new ValidationException(err);
                }

                if (model.Id == 0)
                {
                    var user = WebSecurity.GetUser(User.Identity.Name);

                    var dbTask = model.ToEntity();
                    var dbUser = _unitOfWork.Repository<User>().Query.FirstOrDefault(f => f.Id.Equals((System.Guid)user.ProviderUserKey));
                    dbTask.User = dbUser;

                    _unitOfWork.Repository<Task>().MarkToAdd(dbTask);
                }
                else
                {
                    var dbTask = _unitOfWork.Repository<Task>().Query.FirstOrDefault(f => f.Id == model.Id);
                    _unitOfWork.Repository<Task>().MarkToUpdate(model.ToEntity(dbTask));
                }

                _unitOfWork.Save();
            }
            catch (System.Exception ex)
            {
                Logger.Error("OnSave", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Json(new { result = "Ok" });
        }


        #region API

        public JsonResult List(GridTaskRequestModel searchModel)
        {
            if (searchModel == null)
                searchModel = new GridTaskRequestModel();

            var response = new TableResult();
            try
            {
                var user = WebSecurity.GetUser(User.Identity.Name);
                var tasks = _unitOfWork.Repository<Task>().Query.Include(u => u.User).AsNoTracking().Where(w => w.UserId.Equals((System.Guid)user.ProviderUserKey));

                //Поиск
                if (searchModel.Filter.IsNow.HasValue && searchModel.Filter.IsNow.Value)
                    tasks = tasks.Where(p => !p.Done.HasValue || p.Done.HasValue && !p.Done.Value);
                if (searchModel.Filter.IsDone.HasValue && searchModel.Filter.IsDone.Value)
                    tasks = tasks.Where(p => p.Done.HasValue && p.Done.Value);
                if (!string.IsNullOrEmpty(searchModel.Filter.Name))
                    tasks = tasks.Where(p => searchModel.Filter.Name.Equals(p.Name, System.StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrEmpty(searchModel.Filter.Description))
                    tasks = tasks.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchModel.Filter.Description));

                // Сортировка
                if (searchModel.Sort.Created.HasValue)
                    tasks = searchModel.Sort.Created.Value ? tasks.OrderBy(p => p.Created) : tasks.OrderByDescending(p => p.Created);
                else if (searchModel.Sort.Importance.HasValue)
                    tasks = searchModel.Sort.Importance.Value ? tasks.OrderBy(p => p.Importance) : tasks.OrderByDescending(p => p.Importance);

                response.Result = tasks.AsEnumerable()
                    .Skip(searchModel.Skip)
                    .Take(searchModel.Take)
                    .Select(p => new TaskModel(p)).ToArray();

                // Считаем общее количество записей
                response.TotalCount = tasks.Count();
            }
            catch (System.Exception ex)
            {
                Logger.Error("OnList", ex);
            }

            return JsonNet(response);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var dbTask = _unitOfWork.Repository<Task>().Query.FirstOrDefault(f => f.Id == id);
                if(dbTask == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                _unitOfWork.Repository<Task>().Delete(dbTask);
                _unitOfWork.Save();
            }
            catch (System.Exception ex)
            {
                Logger.Error("OnDelete", ex);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public JsonResult Importance()
        {
            var response = new TableResult();

            try
            {
                response.Result = System.Enum.GetNames(typeof(ImportanceLevel)).AsEnumerable().Select(p => new { title = p }).ToArray();
                response.TotalCount = System.Enum.GetNames(typeof(ImportanceLevel)).Count();
            }
            catch (System.Exception ex)
            {
                Logger.Error("OnImportance", ex);
            }

            return JsonNet(response);
        }


        #endregion API



    }
}