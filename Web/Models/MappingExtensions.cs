namespace YATDL
{
    public static class MappingExtensions
    {
        public static TaskModel ToModel(this Task entity, TaskModel model = null)
        {
            if (model == null)
                model = new TaskModel();

            model.Id = entity.Id;
            model.Done = entity.Done;
            model.Importance = entity.Importance.ToString();
            model.Description = entity.Description;
            model.Name = entity.Name;
            model.Created = entity.Created;

            return model;
        }

        public static Task ToEntity(this TaskModel model, Task entity = null)
        {
            if (entity == null)
                entity = new Task();

            entity.Name = model.Name;
            entity.Done = model.Done;
            entity.Description = model.Description;
            entity.Created = model.Created;

            ImportanceLevel importance;
            System.Enum.TryParse(model.Importance, true, out importance);
            entity.Importance = importance;

            return entity;
        }
    }
}