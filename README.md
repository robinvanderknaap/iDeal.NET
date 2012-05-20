# iDeal.NET
iDeal is the leading online payment platform in the Netherlands. iDeal.NET provides an API to easily communicate with your iDeal provider and integrate iDeal payments into your .NET (web)applications.
The project contains a sample application which gives a basic example of the usage of iDeal.NET. A live version of the sample application is available [here](http://ideal.webpirates.nl). The sample application uses [www.ideal-simulator.nl](http://www.ideal-simulator.nl) to simulate the entire process of paying and retrieving status of payments.

## iDeal versions
iDeal.NET is aimed at iDeal Professional (Rabobank), iDeal Zelfbouw (ABN Amro), iDeal Integrated and iDeal Advanced (ING Bank). These versions allow for realtime feedback on transactions. 
iDeal.NET does not yet support iDeal Basic (ING Bank), iDeal Hosted , iDeal Lite (Rabobank) and iDeal Zakelijk which are easily implemented in applications but do not allow for realtime feeback on transactions.

## NuGet
The easiest way to get started with iDeal.NET is to use the NuGet package

	Install-Package iDeal.NET

NuGet packages are available for .NET 3.5 and 4.0 applications. You can also build the library from source and reference the iDeal.NET.dll in your applications.

## Configuration
By default iDeal.NET is configured through the web.config or app.config.

First declare the configurationsection

	<configSections>
      <section name="iDeal" type="iDeal.Configuration.ConfigurationSectionHandler, iDeal" allowLocation="true" allowDefinition="Everywhere" />
    </configSections>
	
Second implement the iDeal section

	<iDeal>
      <merchant id="123456789" subId="0" />
      <acquirer url="https://www.ideal-simulator.nl:443/professional/" />
      <certificate filename="App_Data\idealsim_private.pfx" password="idealsim" />
      <bankCertificate filename="App_Data\idealsim_bank.cer" />
    </iDeal>

The merchant id is the unique identifier you received from your iDeal provider(acquirer). The acquirer url points to the url of your acquirer which handles all iDeal requests. Certificate filename specifies the relative path to the file containing your private key. The bank certificate filename points to your public key. It's also possible to specifiy a certificate in your certificate store, this is explained below.

## Directory request
In order for customers to make a payment, they first have to choose their bank. To retrieve a list of banks (issuers) which consumers can choose from, you have to send a directory request to your iDeal provider (acquirer). This is how you send a directory request with iDeal.NET:

	var iDealService = new iDealService();
	var directoryResponse = iDealService.SendDirectoryRequest();
	var issuers = directoryResponse.Issuers;

The response from a directory request holds a list of issuers. De issuer object holds all the relevant information of an issuer like id, name and listtype. The listtype determines if the issuer should be placed on the shortlist or the longlist. Issuers on the shortlist are listed first in the dropdown, on longlist second. Check the iDeal Merchant Integration Guide from your acquirer for exact details of rendering the dropdown.
In order the minimize the calls to the acquirer it's recommended (and often required) you cache the result of a directory request and refresh the cache every 24 hours.

## Transaction request
When a customer has choosen an issuer you can send a transaction request to the selected issuer:

	// Send transaction request to selected issuer
	var transactionResponse = _iDealService.SendTransactionRequest(
		issuerId: 3, 
		merchantReturnUrl: "http://www.webpirates.nl/landingpage", 
		purchaseId: "12345", 
		amount: 500, 
		expirationPeriod: TimeSpan.FromMinutes(5), 
		description: "Some description",
		entranceCode: "67890";

	// Redirect user to transaction page of issuer
	Response.Redirect(transactionResponse.IssuerAuthenticationUrl);

The following parameters have to be specified to perform a transaction request
 
 - issuerId: Unique identifier of the selected issuer
 - merchantReturnUrl: Url to which the customers is redirected after the paymentproces finishes
 - purchaseId: Unique identifier of payment
 - amount: The amount in cents
 - expirationPeriod: Number of minutes before payment expires
 - description: Description of the payment, will be shown on customer's bankstatement.
 - entranceCode: ...
 	
The response of a transactionrequst holds the following information:

 - TransactionId: Uniquely identifies the transaction, used to retrieve the status of a transaction
 - IssuerAuthenticationUrl: Url of the selected issuer to which you have to redirect the customer to make the payment
 - PurchaseId: Id to identify the payment
 
When the response from your acquirer is received you can redirect the customer to the issuer who will be performing the iDeal payment (IssuerAuthenticationUrl). It's important to store the transaction id, you will need this to retrieve the status on the landingpage you specified to which the customer will be redirected when finishing the payment.

## Status request
When a customer is redirected back from the issuer after the payment finishes you can perform a status request to retrieve the status of the transaction:

	var iDealService = new iDealService();
	var statusResponse = iDealService.SendStatusRequest(transactionId);

## Certificates

## License
All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)