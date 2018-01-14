declare const __API__: string;

export default class MainService {
    static fetch(action: string, method: string, obj?: any, mode?: string): Promise<Response> {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return fetch(__API__ + action, {
            body: obj,
            method: method,
            headers: headers,
            mode: "no-cors"
        });
    }
}