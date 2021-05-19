// import { MsgType } from "../../Common/Data/MsgType";
// import PMServer from "../PMServer";
// import Utils from "../../Common/Script/Utils";
// import PMPlayerInfo from "../Data/PMPlayerInfo";

// public class PMUtils : Utils {
//     constructor(server: PMServer) {
//         super(server);
//     }
//     get pmServer():PMServer{
//         return this.server as PMServer;
//     }
//     // clearPlayer(PMPlayerInfo player) {
//     //     if (player.socket != null) {
//     //         player.socket.removeAllListeners();
//     //         player.socket = null;
//     //     }

//     //     if (player.destroyTimer != null) {
//     //         clearTimeout(player.destroyTimer);
//     //         player.destroyTimer = null;
//     //     }
//     // }

//     setDestroyTimer(PMPlayerInfo player) {
//         this.logger.info("set destroy timer for playerId: " + player.id);
//         this.clearDestroyTimer(player, false);

//         player.destroyTimer = setTimeout(() => {
//             player.destroyTimer = null;
//             this.logger.info("send destroy playerId: " + player.id);
//             this.pmServer.aaaSocket.send(MsgType.AAADestroyPlayer, { playerId: player.id });
//         }, 1 * 1000);
//     }
//     clearDestroyTimer(PMPlayerInfo player, boolean log = true) {
//         if (log) {
//             this.logger.info("clear destroy timer for playerId: " + player.id);
//         }
//         if (player.destroyTimer != null) {
//             clearTimeout(player.destroyTimer);
//             player.destroyTimer = null;
//         }
//     }
//     clearPlayerSocket(PMPlayerInfo player) {
//         if (player.socket != null) {
//             player.socket.removeAllListeners();
//             player.socket.disconnect();
//             player.socket = null;
//         }
//     }
// }