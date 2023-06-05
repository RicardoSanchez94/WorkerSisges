using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using WorkerServiceSisges.Clases;

namespace WorkerCauCapa.Model.Clases
{
    public class EnviodeCorreo
    {
        public void ErrorApi(ResponseModel Texto)
        {
            var response = new ResponseModel();
            try
            {


                var asunto = "Error en el WorkerSisges";
                String textoEmail = "";
                SmtpClient client = new SmtpClient("smtp.office365.com", 587);


                client.EnableSsl = true;
                client.Timeout = 1000000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // nos autenticamos con nuestra cuenta de gmail

                //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                client.Credentials = new NetworkCredential("sisges.cuadratura@fashionspark.com", "Cat03082");//cambio contraseña 
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("patricio.meneses@fashionspark.com"));
                mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                //mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));

                mail.From = new MailAddress("sisges.cuadratura@fashionspark.com");
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;

         



      
                if (Texto.error)
                {
                    textoEmail = "  <strong> Estimados el Worker a Presentado un error a las: " + DateTime.Now + " </strong><br>" +
                    "<div style= text-align: justify;>ExcepcionMensaje :" + Texto.respuesta + "<br>" +
                    "ExcepInnerException: " + "</div>";
                }
                else
                {
                    textoEmail = "  <strong> Estimados el worker ha Insertado Correctamente a las : " + DateTime.Now + " </strong><br>" +
                   "<div style= text-align: justify;>ExcepcionMensaje :" + Texto.respuesta + "<br>" +
                   "ExcepInnerException: " + "</div>";
                }


                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                mail.BodyEncoding = UTF8Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                mail.AlternateViews.Add(htmlView);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
 
                client.Send(mail);

                response.error = false;
                response.respuesta = "CORREO ENVIADO CORRECTAMENTE";

  
         


            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "ERROR: " + ex.Message;
            }




        }
    }
}
