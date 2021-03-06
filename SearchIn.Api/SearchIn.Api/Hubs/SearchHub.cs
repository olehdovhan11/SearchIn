﻿using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchIn.Api.Services;
using SearchIn.Api.Models;
using SearchIn.Api.Exceptions;

namespace SearchIn.Api.Hubs
{
	public class SearchHub: Hub<IClient>
	{
		private ISearchService searchService;

		public SearchHub(ISearchService searchService)
		{
			this.searchService = searchService;
			this.searchService.UrlStateChanged += UrlStateChangedHandler;
			this.searchService.NewUrlListFound += NewUrlListFoundHandler;
		}

		public override Task OnConnected()
		{
			Clients.Caller.onConnected();
			return base.OnConnected();
		}

		public void StartSearch(string startUrl, string searchString, int countUrls, int countThreads)
		{
			Task.Run(() =>
			{
				try
				{
					searchService.StartSearch(startUrl, searchString, countUrls, countThreads);
				}
				catch (SearchProcessException ex)
				{
					SendErrorMessageToClient(ex.Message);
				}
			});
		}
		public void PauseSearch()
		{
			try
			{
				searchService.PauseSearch();
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}
		public void ResumeSearch()
		{
			try
			{
				searchService.ResumeSearch();
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}
		public void StopSearch()
		{
			try
			{
				searchService.StopSearch();
			}
			catch (SearchProcessException ex)
			{
				SendErrorMessageToClient(ex.Message);
			}
		}

		private void UrlStateChangedHandler(UrlStateDto urlStateDto)
		{
			Clients.Caller.onUrlStateChanged(urlStateDto);
		}
		private void NewUrlListFoundHandler(IEnumerable<UrlDto> urlList)
		{
			Clients.Caller.onNewUrlListFound(urlList);
		}

		private void SendErrorMessageToClient(string errorMessage)
		{
			Clients.Caller.onErrorFound(errorMessage);
		}
	}
}