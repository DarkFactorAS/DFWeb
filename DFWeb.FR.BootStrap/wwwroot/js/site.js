
    //
    // Internal class to post data
    //
    function postData(url, postData)
    {
        // Show some debug
        const sData = JSON.stringify(postData);
        console.log("postData:" + url + " => " + sData);

        fetch(url,
        {
            method: 'POST',
            headers:
            {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: sData
        })
        .then(async function(response)
        {
            const responseText = await response.text();

            if (!response.ok)
            {
                throw new Error(responseText || response.statusText);
            }

            console.debug("PostData[OK]:" + responseText);
            window.location.reload();
        })
        .catch(function(error)
        {
            console.debug("PostData[ERROR]:" + error);
        });
    }
    
    function getSectionValue(sectionId,fieldName) 
    { 
        const field = document.getElementById(fieldName + "_" + sectionId);
        return field ? field.value : null;
    } 

    function getFieldValue(fieldName) 
    { 
        const field = document.getElementById(fieldName);
        return field ? field.value : null;
    } 
