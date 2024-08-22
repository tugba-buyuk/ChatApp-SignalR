$(function () {
    const connection= new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5253/messagehub")
            .withAutomaticReconnect([1000,2000,4000]) 
            .build();

        connection.start();

        $(".disabled").attr("disabled", "disabled");
        $("body").on("click", ".users", function () {
         
          $(".users").each((index, item) => {
            item.classList.remove("active");
          });
          $(this).addClass("active");
        });

        $("#btnLogin").on("click" ,()=>{
            const nickName= $("#txtNickName").val();
            connection.invoke("GetNickName",nickName).catch(err=>console.log(`Ocurred a error while sending nickname: ${err}`));
            $(".disabled").removeAttr("disabled");
        });

        connection.on("clientJoined",nickName=>{
            $("#clientStatusMessages").html(`${nickName} joined us.`);
            $("#clientStatusMessages").fadeIn(2000,()=>{
                setTimeout(()=>{
                    $("#clientStatusMessages").fadeOut(2000);
                },2000);
            });
        });

        connection.on("clientList",clientList=>{
            $("#_clients").html("");
            $.each(clientList,(index,item)=>{
                const user=$(".users").first().clone();
                user.removeClass("active");
                user.html(item.nickName);
                $("#_clients").append(user);
            });
        });

        $("#btnSend").on("click",()=>{
            const clientName=$(".users.active").first().html();
            const message=$("#txtMessage").val();
            connection.invoke("SendMessageAsync",clientName,message);

            const messageItem= $(".message-item").clone();
            const messageList=$(".messageGroup");
            messageItem.removeClass("message-item");
            messageItem.find("p").html(message);
            messageItem.find("h5")[1].innerHTML="sen";
            messageList.append(messageItem);
        });

        connection.on("receiveMessage",(message,senderClient)=>{
            const messageItem= $(".message-item").clone();
            const messageList=$(".messageGroup");
            messageItem.removeClass("message-item");
            messageItem.find("p").html(message);
            messageItem.find("h5")[0].innerHTML=senderClient;
            messageList.append(messageItem);
        });

        $("#btnCreateGroup").on("click",()=>{
            const groupName=$("#groupName").val();
            connection.invoke("AddGroup",groupName);
        })

        connection.on("groups",groupList=>{
            console.log(groupList);
            let options="";
            $.each(groupList,(index,item)=>{
                options += `<option value="${item.groupName}">${item.groupName}</option>`;
                console.log(options);
            })
            $(".rooms").append(options);
        });

        $("#btnJoinGroups").on("click",()=>{
            const groupNames=[];
            $(".rooms option:selected").map((index,element) =>
                groupNames.push(element.innerHTML));
            connection.invoke("AddClientIntoGroup",groupNames);
        });

        $(".rooms").on("change",function()
        {
            const gName=$(this).val()[0];
            connection.invoke("GetClientsForGroup",gName);
        })
            

});