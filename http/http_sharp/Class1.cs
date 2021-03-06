﻿using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

namespace FM.WebServer
{
    public delegate SimplSharpString WebServerRequestDelegate(SimplSharpString path);

    public class WebServer
    {
        int ID = 12;

        public int GetID()
        {
            CrestronConsole.PrintLine("Sample simplsharp callback");
            return ID;
        }

        #region Class variables
        HttpServer server;
        int port;
        #endregion

        public WebServerRequestDelegate RequestCallback { get; set; }

        #region Properties
        public int TraceEnabled { get; set; }
        public string TraceName { get; set; }
        #endregion

        #region Constants
        const string ServerName = "FM SimplSharp WebServer";
        #endregion

        #region Constructor
        public WebServer()
        {
            this.TraceName = this.GetType().Name;
            this.port = 7000;
        }
        #endregion

        #region Public methods
        public int StartListening()
        {
            try
            {
                if (server == null)
                {
                    Trace("StartListening() Creating new HttpServer object.");

                    server = new HttpServer();
                    server.Port = port;
                    server.ServerName = ServerName;
                    server.OnHttpRequest += new OnHttpRequestHandler(ServerHttpRequestHandler);
                    server.Open();

                    return 1;
                }
                else
                {
                    Trace("StartListening() server object already exists. No action taken.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace("StartListening() exception caught: " + ex.Message);
                return 0;
            }
        }
        public bool StopListening()
        {
            try
            {
                if (server != null)
                {
                    server.Close();
                    server.Dispose();
                    server = null;
                    return true;
                }
                else
                {
                    Trace("StopListening() server object is already null. No action taken.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace("StopListening() exception caught: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Private methods
        void Trace(string message)
        {
            if (TraceEnabled == 1)
                CrestronConsole.PrintLine(String.Format("[{0}] {1}", TraceName, message.Trim()));
        }
        SimplSharpString RequestCallbackNotify(string path)
        {
            if (RequestCallback != null)
                return RequestCallback(path);
            else
                Trace("RequestCallbackNotify() callback is not defined.");
            return("no data");
        }
        #endregion

        #region Event callbacks
        void ServerHttpRequestHandler(object sender, OnHttpRequestArgs e)
        {
            Trace("ServerHttpRequestHandler() received request. Path: " + e.Request.Path);
            SimplSharpString answer = RequestCallbackNotify(e.Request.Path);
            CrestronConsole.PrintLine("ANSWER: " + answer.ToString());
            e.Response.ContentString = answer.ToString();
        }
        #endregion
    }
}
