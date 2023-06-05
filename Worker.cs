using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkerCauCapa.Model.Clases;
using WorkerServiceSisges.Clases;

namespace WorkerServiceSisges
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private DateTime _lastExecutionDate = DateTime.MinValue;
        private EnviodeCorreo Correo = new EnviodeCorreo();




        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        // await StarProceso();
        //        try
        //        {
        //            await IniciodeProceso();
        //            _logger.LogInformation("Se finalizo Correctamente");
        //        }
        //        catch (Exception ex)
        //        {

        //            _logger.LogInformation("Error en el ExecuteASync", ex);
        //        }
        //        finally
        //        {
        //            _logger.LogInformation("Fin del ExecuteAsync ");
        //            await Task.Delay(10000000, stoppingToken);
        //            //await Task.Delay(30000000, stoppingToken);

        //        }


        //    }
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var nextSeven = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);
            var nextEleven = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);


            ResponseModel response = new ResponseModel();
            if (now.Hour == nextSeven.Hour)
            {
                try
                {
                    await IniciodeProceso();
                    _logger.LogInformation("Se finalizo Correctamente el proceso de las 7");
                }
                catch (Exception ex)
                {

                    _logger.LogInformation("Error en el Proceso de las 7", ex);
                }
                finally
                {
                    _logger.LogInformation("Fin del ExecuteAsync ");
                    await Task.Delay(10000000, stoppingToken);
                }

            }
            else
            {
                nextSeven = nextSeven.AddDays(1);
            }
            if (now.Hour == nextEleven.Hour)
            {
            
                try
                {
                    await IniciodeProceso();
                    response.error = false;
                    _logger.LogInformation("Se finalizo Correctamente el proceso de las 11");
                }
                catch (Exception ex)
                {
                    response.error = true;
                    Correo.ErrorApi(response);
                    _logger.LogInformation("Error en el Proceso de las 11", ex);
                }
                finally
                {
                    _logger.LogInformation("Fin del ExecuteAsync ");

                    while (stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }

            }
            else
            {
                //nextEleven = nextEleven.AddDays(1);
                //while (!stoppingToken.IsCancellationRequested)
                //{
                _logger.LogInformation("Fin del ExecuteAsync ");
                await Task.Delay(100000000, stoppingToken);
                 
                //}
            }



        }
     


        #region CauuCAPA
        public async Task<List<ResponseModel>> IniciodeProceso()
        {
            ResponseModel response = new ResponseModel();
            List<ResponseModel> lstresponse = new List<ResponseModel>();
            DateTime Fecha = DateTime.Today.AddDays(-2);
            //DateTime Fecha = DateTime.Today.AddDays(-1);
            //_logger.LogInformation("Inicio de proceso con la fecha : " + Fecha.ToString("dd-MM-yyyy"));
            try
            {
                response = await GeneraTOKEN();
                lstresponse.Add(response);
                if (!response.error)
                {
                    //11 am
                    if (DateTime.Now.Hour == 7)
                    {
                        //7 am
                        response = await GenerarCAUUCAPA(response.Token, Fecha);
                        lstresponse.Add(response);
                    }

                    if (DateTime.Now.Hour == 11)
                    {
                        response = await EndpointAlertas(response.Token);
                        lstresponse.Add(response);
                    }
               


                }
                 response.error = false;
                response.respuesta = "Termina el proceso de Generacion";
                Correo.ErrorApi(response);
                _logger.LogInformation("Termina el proceso de Generacion");
                return lstresponse;


            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api";
                Correo.ErrorApi(response);
               
                _logger.LogInformation("Error al ejecutar la api" + ex.Message);

                return lstresponse;
            }
       
        }

      

        public async Task<ResponseModel> GeneraTOKEN()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var Credeenciales = Helper.Endpoints();

                var url = string.Format(Credeenciales.BaseUrl + Credeenciales.Endpoints.Token);
                var parametros = new
                {
                    rut = Credeenciales.Endpoints.Rut,
                    pass = Credeenciales.Endpoints.Pass,
                 
                };

                var jsonP = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
                var content = new StringContent(jsonP, Encoding.UTF8, "application/json");
                using (HttpClient cliente = new HttpClient())
                {


                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var request = await cliente.PostAsync(url, content);
                    _logger.LogInformation("Se conecto Correctamente a la API");
                    var contenido = await request.Content.ReadAsStringAsync();
                    var Json = JsonConvert.DeserializeObject<ApiSiges>(contenido);
                    response.Token = Json.result;
                    response.respuesta = "Genero el Token Correctamente";
                    _logger.LogInformation("Genero el Token Correctamente");
                    response.error = false;

                    return response;
                }
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas" + ex.Message;
                Correo.ErrorApi(response);
                _logger.LogInformation("Error al ejecutar la Api de Alertas" + ex.Message);

                return response;
            }

        }




        //public async Task<ResponseModel> GenerarCAUUCAPA(string Token,DateTime Fecha)
        //{
        //    ResponseModel response = new ResponseModel();

        //    try
        //    {
        //        var Credeenciales = Helper.Endpoints();
        //        var url = string.Format(Credeenciales.BaseUrl + Credeenciales.Endpoints.LecturaArchivosSFT);
        //        var parametros = new
        //        {
        //            Fecha= Fecha,


        //        };

        //        var jsonP = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
        //        var content = new StringContent(jsonP, Encoding.UTF8, "application/json");
        //        var Bearer = string.Format("Bearer " + Token);
        //        using (HttpClient cliente = new HttpClient())
        //        {
        //            _logger.LogInformation("Se conecto Correctamente a la API");
        //            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        //            requestMessage.Headers.Add("Authorization", Bearer); 
        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //            cliente.Timeout = Timeout.InfiniteTimeSpan;
        //            var respuesta = await cliente.SendAsync(requestMessage);
        //            var contenido = await respuesta.Content.ReadAsStringAsync();
        //            //var Json = JsonConvert.DeserializeObject<ApiSiges>(contenido);
        //            //response.Token = Json.result;
        //            response.respuesta = "Genero Interfaces";
        //            _logger.LogInformation("Genero Interfaces");
        //            response.error = false;

        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        response.error = true;
        //        response.respuesta = "Error al ejecutar la Api de Alertas" + ex.Message;
        //        _logger.LogInformation("Error al ejecutar la Api de Alertas" + ex.Message);

        //        return response;
        //    }

        //}
        public async Task<ResponseModel> GenerarCAUUCAPA(string Token, DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var Credeenciales = Helper.Endpoints();
                var url = string.Format(Credeenciales.BaseUrl + Credeenciales.Endpoints.LecturaArchivosSFT);
                var parametros = new
                {
                    Fecha = Fecha
                };

                var jsonP = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
                var content = new StringContent(jsonP, Encoding.UTF8, "application/json");
                var Bearer = string.Format("Bearer " + Token);

                using (HttpClient httpClient = new HttpClient())
                {
                    _logger.LogInformation("Se conecto Correctamente a la API");

                    httpClient.DefaultRequestHeaders.Add("Authorization", Bearer);

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                    requestMessage.Content = content;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    httpClient.Timeout = Timeout.InfiniteTimeSpan;

                    var respuesta = await httpClient.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();

                    response.respuesta = "Genero Interfaces";
                    _logger.LogInformation("Genero Interfaces");
                    response.error = false;

                    return response;
                }
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas" + ex.Message;
                _logger.LogInformation("Error al ejecutar la Api de Alertas" + ex.Message);

                return response;
            }
        }



        #endregion

        #region AlertasSencillo


        public async Task<ResponseModel> EndpointAlertas(string Token)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var Credeenciales = Helper.Endpoints();
                var url = string.Format(Credeenciales.BaseUrl + Credeenciales.Endpoints.AlertasSencillo);
                //var url = Credeenciales.Endpoints.LecturaArchivosSFT;
                var Bearer = string.Format("Bearer " + Token);
                using (HttpClient cliente = new HttpClient())
                {
                    _logger.LogInformation("Se conecto Correctamente a la API");
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Authorization", Bearer);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var respuesta = await cliente.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    //var Json = JsonConvert.DeserializeObject<ApiSiges>(contenido);
                    //response.Token = Json.result;
                    response.respuesta = "Genero Alertas";
                    _logger.LogInformation("Genero Alertas");
                    response.error = false;

                    return response;
                }
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas" + ex.Message;
                _logger.LogInformation("Error al ejecutar la Api de Alertas" + ex.Message);

                return response;
            }

        }

        #endregion
    }

}
