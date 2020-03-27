> nothing
> 有空就更

![avatar](IngestSplash.png)

- Ingest
  - **IngestDB**
  - IngestMsg
  - IngestTask

  ```
  Scaffold-DbContext "Server=172.16.0.205;Database=ingestdb;port=3307;uid=sdba;pwd=sdba;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -Context IngestGlobalDBContext -Project IngestGlobalPlugin -Tables dbp_captureparamtemplate,dbp_captureparamtemplate_map,dbp_fileformatinfo,dbp_global,dbp_global_program,dbp_global_state -f
  ```

  * 尽量把查询语句写成一句话在`Store`执行，避免重复查询数据库导致的效率问题(###`Linq`使用效率很重要###)
  * 尽量使用`SobeyRecException`来抛出异常，好返回前端详细错误以及code（###注意param###）
  * 尽量使用`Automap`来转化新老接口返回数据不一致问题
  * 内部通信请使用相应的`Interface`接口
  * 统一使用`linq xml`增大效率
  * 老数据结构的`Arrayy`一律改成`List`