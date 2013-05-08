# iDeal.NET
iDeal is the leading online payment platform in the Netherlands. iDeal.NET provides an API to easily communicate with your iDeal provider and integrate iDeal payments into your .NET (web)applications.
The project contains a sample application which gives a basic example of the usage of iDeal.NET. A live version of the sample application is available [here](http://ideal.skaele.it). The sample application uses [www.ideal-simulator.nl](http://www.ideal-simulator.nl) to simulate the entire process of paying and retrieving status of payments.

## iDeal versions
iDeal.NET is aimed at iDeal Professional (Rabobank), iDeal Zelfbouw (ABN Amro), iDeal Integrated and iDeal Advanced (ING Bank). These versions allow for real-time feedback on transactions. 
iDeal.NET does not yet support iDeal Basic (ING Bank), iDeal Hosted , iDeal Lite (Rabobank) and iDeal Zakelijk which are easily implemented in applications but do not allow for real-time feedback on transactions.

## NuGet
The easiest way to get started with iDeal.NET is to use the NuGet package

	Install-Package iDeal.NET

NuGet packages are available for .NET 3.5 and 4.0 applications. Binaries are available for download [here](https://github.com/robinvanderknaap/iDeal.NET/downloads). You can also build the library from source and reference the iDeal.dll in your applications.


## Configuration
By default iDeal.NET is configured through the web.config or app.config.

First declare the configuration section

	<configSections>
      <section name="iDeal" type="iDeal.Configuration.ConfigurationSectionHandler, iDeal" allowLocation="true" allowDefinition="Everywhere" />
    </configSections>
	
Second implement the iDeal section

	<iDeal>
        <merchant id="123456789" subId="0" />
        <acquirer url="https://www.ideal-simulator.nl:443/professional/" />
        <acceptantCertificate filename="Util\TestCertificates\idealsim_private.pfx" password="idealsim" />
		<acquirerCertificate filename="Util\TestCertificates\idealsim_public.cer" />
    </iDeal>

The merchant id is the unique identifier you received from your iDeal provider(acquirer). The merchant subId is usually 0, or otherwise specified by the acquirer. The url points to the url of your acquirer which handles all iDeal requests.

The acceptant certicate is the private certificate created or bought by the acceptant (webshop), the related public key has to be uploaded to your acquirer. See below how to create a new self signed certificate. 

The acquirer certificate is the certificate you receive from your ideal provider, with this certificate responses from the acquirer are verified.

The filenames of the certificates specify the relative path to the files containing the certificates. Only for the private acceptant certificate a password is needed.

It's also possible to load the certificates from a certificate store, which is the most secure and preferable option:
	
	<iDeal>
        <merchant id="123456789" subId="0" />
        <acquirer url="https://www.ideal-simulator.nl:443/professional/" />
        <acceptantCertificate storeLocation="LocalMachine" storeName="My" thumbprint="48 fa ca 26 2a 9f 76 66 67 f0 bf 2f ed 54 b8 db 16 f9 10 87" />
		<acquirerCertificate storeLocation="LocalMachine" storeName="My" thumbprint="6c fc 36 38 9c 7a 3c 49 44 0b 87 33 d2 58 cb 21 67 fa c1 8f" />
    </iDeal>

The store location has to be specified, which can be 'LocalMachine' or 'CurrentUser'. The name of the store also has to be specified. The thumbprint is used to uniquely identify the certificate. When you open your certificate in the certificate store (using the management console) you can view the thumbprint (right-click certificate in management console, choose 'open', select the 'details' tab, find 'Thumbprint' in the list of fields). Just copy-paste the thumbprint (iDeal.NET will remove the spaces and is case insensitive).
No password is needed for the acceptant's certificate, that's only needed when the certificate is loaded from the filesystem.

## Directory request
In order for customers to make a payment, they first have to choose their bank. To retrieve a list of banks (issuers) which consumers can choose from, you have to send a directory request to your iDeal provider (acquirer). This is how you send a directory request with iDeal.NET:

	var iDealService = new iDealService();
	var directoryResponse = iDealService.SendDirectoryRequest();
	var issuers = directoryResponse.Issuers;

The response from a directory request holds a list of issuers. De issuer object holds all the relevant information of an issuer like id, name and listtype. The listtype determines if the issuer should be placed on the shortlist or the longlist. Issuers on the shortlist are listed first in the dropdown, on the longlist second. Check the iDeal Merchant Integration Guide from your acquirer for exact details on rendering the dropdown.
In order the minimize the calls to the acquirer it's recommended (and often required) you cache the result of a directory request and refresh the cache every 24 hours.

## Transaction request
When a customer has choosen an issuer you can send a transaction request to the selected issuer:

	// Send transaction request to selected issuer
	var transactionResponse = _iDealService.SendTransactionRequest(
		issuerId: 3, 
		merchantReturnUrl: "http://www.skaele.nl/landingpage", 
		purchaseId: "12345", 
		amount: 500, 
		expirationPeriod: TimeSpan.FromMinutes(5), 
		description: "Some description",
		entranceCode: "67890";

	// Redirect user to transaction page of issuer
	Response.Redirect(transactionResponse.IssuerAuthenticationUrl);

The following parameters have to be specified to perform a transaction request
 
 - issuerId: Unique identifier of the selected issuer
 - merchantReturnUrl: Url to which the customers is redirected after the payment process finishes
 - purchaseId: Unique identifier generated by merchant/acceptant
 - amount: The amount in cents (euro)
 - expirationPeriod: Period consumer has to finish the tranaction before it is marked as expired by the issuer
 - description: Description of the payment, will be shown on customer's bank statement.
 - entranceCode: Unique code to identify consumer when returning to webshop, generated by merchant/acceptant
 	
The response of a transaction request holds the following information:

 - TransactionId: Uniquely identifies the transaction, used to retrieve the status of a transaction
 - IssuerAuthenticationUrl: Url of the selected issuer to which you have to redirect the customer to make the payment
 - PurchaseId: Unique identifier generated by merchant/acceptant in the transaction request
 
When the response from your acquirer is received you can redirect the customer to the issuer who will be performing the iDeal payment (IssuerAuthenticationUrl). It's important to store the transaction id, you will need this to retrieve the status on the landing page you specified to which the customer will be redirected when finishing the payment.

## Status request
When a customer has finished the payment, the customer is redirected back to the url you specified when sending the transaction request. The issuer will add two query parameters to the url 'trxid' and 'ec'. Something like: http://your-url.nl/Status?trxid=0000000000078401&ec=ce6462a2-ce87-46
Parameter 'trxid' holds the transaction id, and 'ec' holds the entrance code you specified in the transaction request. With the transaction id you can retrieve the status of the payment:

	var iDealService = new iDealService();
	var statusResponse = iDealService.SendStatusRequest(transactionId);
	
The response contains the status, which can be Success, Failure, Cancelled, Open or Expired. The response also contains the account number, name and city of the customer.

## Certificates
In order to use iDeal you need to create (or buy) a ssl certificate. The public key needs to be uploaded to your iDeal provider (acquirer) so they are able to verify your messages/requests. To create a self-signed certifcate you can use [MakeCert](http://msdn.microsoft.com/en-us/library/bfsktky3.aspx) (use Visual Studio 2010 command prompt) to create a private key and store it into the certificate store, for example:

    makecert -r -pe -n "CN=yourCertificateName" -b 01/01/2000 -e 01/01/2036 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr LocalMachine -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12
	
This creates a new self-signed certificate which is valid until january 1st 2036. The store location is LocalMachine, the store is called My. You can view this certificate in the management console (run -> mmc). You have to add the certificates module to your console to be able to view the certificate. You can also use the management console to export the public key and, if you need it, also export the private key.

When deploying your application, IIS needs to have permission to access the certificates. It's best to store the certificate in the LocalMachine certificate store. You can right click the certificate in the MMC store, choose 'Manage private keys' to grant access to application pool user.

You will receive a public key from your iDeal provider, with which you can verify responses from the provider. You should also store this certificate in a certificate store, you can import the certificate from the acquirer using the management console.


## License
All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)