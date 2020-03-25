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