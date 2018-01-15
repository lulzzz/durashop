declare const __API__: string;

export default class MainService {
    static fetch(useApiUri: boolean, action: string, method: string, obj?: any, mode?: string): Promise<Response> {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return fetch(useApiUri ? __API__ + action : action, {
            body: obj,
            method: method,
            headers: headers
        }).then((response) => {
            if (!response.ok) throw new Error(response.statusText);

            return response;
        });
    }
}