/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import axios, { AxiosInstance, AxiosRequestConfig } from "axios"

import EngineRunner, { getDefaultEngineRunner } from "@core/services/engine/EngineRunner"
import EngineUrl from "@core/services/engine/EngineUrl"

class Engine {
	private readonly client: AxiosInstance
	private runner: EngineRunner

	constructor() {
		this.client = axios.create({
			headers: {
				"Content-Type": "application/x-www-form-urlencoded;charset=utf-8",
			},
			validateStatus: Engine.validateStatus,
			withCredentials: true,
		})
		this.runner = getDefaultEngineRunner()
	}

	private static getEndpoint(endpointOrUrl: string | EngineUrl): string {
		return typeof endpointOrUrl === "string" ? endpointOrUrl : endpointOrUrl.toString()
	}

	private static validateStatus(status: number): boolean {
		return status >= 200 && status <= 503
	}

	public setRunner(runner?: EngineRunner): void {
		this.runner = !(typeof runner === "undefined" || runner === null) ? runner : getDefaultEngineRunner()
	}

	public get<TModel>(endpoint: string, config?: AxiosRequestConfig): Promise<TModel | undefined>
	public get<TModel>(url: EngineUrl, config?: AxiosRequestConfig): Promise<TModel | undefined>
	public get<TModel>(endpointOrUrl: string | EngineUrl, config?: AxiosRequestConfig): Promise<TModel | undefined> {
		return this.runner(this.client.get, Engine.getEndpoint(endpointOrUrl), config)
	}

	public post<TData, TModel>(
		endpoint: string,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public post<TData, TModel>(
		url: EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public post<TData, TModel>(
		endpointOrUrl: string | EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined> {
		return this.runner(this.client.post, Engine.getEndpoint(endpointOrUrl), data, config)
	}

	public patch<TData, TModel>(
		endpoint: string,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public patch<TData, TModel>(
		url: EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public patch<TData, TModel>(
		endpointOrUrl: string | EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined> {
		return this.runner(this.client.patch, Engine.getEndpoint(endpointOrUrl), data, config)
	}

	public put<TData, TModel>(
		endpoint: string,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public put<TData, TModel>(
		url: EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined>
	public put<TData, TModel>(
		endpointOrUrl: string | EngineUrl,
		data?: TData | any,
		config?: AxiosRequestConfig
	): Promise<TModel | undefined> {
		return this.runner(this.client.put, Engine.getEndpoint(endpointOrUrl), data, config)
	}

	public delete<TModel>(endpoint: string, config?: AxiosRequestConfig): Promise<TModel>
	public delete<TModel>(url: EngineUrl, config?: AxiosRequestConfig): Promise<TModel>
	public delete<TModel>(endpointOrUrl: string | EngineUrl, config?: AxiosRequestConfig): Promise<TModel | undefined> {
		return this.runner(this.client.delete, Engine.getEndpoint(endpointOrUrl), config)
	}
}

export default Engine
